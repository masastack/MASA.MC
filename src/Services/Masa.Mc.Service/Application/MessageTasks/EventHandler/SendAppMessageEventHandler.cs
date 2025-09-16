// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendAppMessageEventHandler
{
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly ILogger<SendAppMessageEventHandler> _logger;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly IAppVendorConfigRepository _appVendorConfigRepository;
    private readonly IAppDeviceTokenRepository _appDeviceTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SendAppMessageEventHandler(
        AppNotificationSenderFactory appNotificationSenderFactory,
        IChannelRepository channelRepository,
        IMessageRecordRepository messageRecordRepository,
        IMessageTaskHistoryRepository messageTaskHistoryRepository,
        ILogger<SendAppMessageEventHandler> logger,
        IWebsiteMessageRepository websiteMessageRepository,
        IAppVendorConfigRepository appVendorConfigRepository,
        IAppDeviceTokenRepository appDeviceTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _appNotificationSenderFactory = appNotificationSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _logger = logger;
        _websiteMessageRepository = websiteMessageRepository;
        _appVendorConfigRepository = appVendorConfigRepository;
        _appDeviceTokenRepository = appDeviceTokenRepository;
        _unitOfWork = unitOfWork;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendAppMessageEvent eto)
    {
        var taskHistory = eto.MessageTaskHistory;

        if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Assign && !taskHistory.ReceiverUsers.Any())
        {
            await SetTaskHistoryStatusAsync(taskHistory, MessageSendStatuses.Fail);
            return;
        }

        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var transmissionContent = GetTransmissionContent(channel.Type, eto.MessageData.MessageContent);

        MessageSendStatuses sendStatus;

        if (channel.Provider == (int)AppChannelProviders.Mc)
        {
            sendStatus = await SendMcAppMessageAsync(eto, transmissionContent);
        }
        else
        {
            sendStatus = await SendThirdPartyAppMessageAsync(eto, channel.Provider, channel.ExtraProperties, transmissionContent, taskHistory);
        }

        await SetTaskHistoryStatusAsync(taskHistory, sendStatus);
    }

    private async Task SetTaskHistoryStatusAsync(MessageTaskHistory taskHistory, MessageSendStatuses status)
    {
        taskHistory.SetResult(status);
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private ExtraPropertyDictionary GetTransmissionContent(ChannelType channelType, MessageContent messageContent)
    {
        var appChannel = channelType as ChannelType.AppsChannel;
        return appChannel?.GetMessageTransmissionContent(messageContent) ?? new ExtraPropertyDictionary();
    }

    private async Task<MessageSendStatuses> SendThirdPartyAppMessageAsync(SendAppMessageEvent eto, int channelProvider, ConcurrentDictionary<string, object> channelProperties, ExtraPropertyDictionary transmissionContent, MessageTaskHistory taskHistory)
    {
        var appSenderProvider = (AppPushProviders)channelProvider;
        var options = _appNotificationSenderFactory.GetOptions(appSenderProvider, channelProperties);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(appSenderProvider);

        using (asyncLocal.Change(options))
        {
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(appSenderProvider);
            return await SendMessageBasedOnReceiverTypeAsync(sender, eto, transmissionContent, taskHistory, (AppPlatform)appSenderProvider, taskHistory.ReceiverUsers);
        }
    }

    private async Task<MessageSendStatuses> SendMessageBasedOnReceiverTypeAsync(
    IAppNotificationSender sender,
    SendAppMessageEvent eto,
    ExtraPropertyDictionary transmissionContent,
    MessageTaskHistory taskHistory,
    AppPlatform platform,
    List<MessageReceiverUser> receiverUsers)
    {
        var receiverType = taskHistory.MessageTask.ReceiverType;
        var isUniformContent = taskHistory.MessageTask.IsUniformContent;

        if (receiverType == ReceiverTypes.Broadcast)
        {
            if (sender.SupportsBroadcast)
            {
                return await HandleBroadcastAsync(sender, eto.MessageData, transmissionContent);
            }

            receiverUsers = await GetReceiverUsersFromDeviceTokensAsync(eto.ChannelId, platform, taskHistory.MessageTask.Variables);
        }

        if (isUniformContent && receiverUsers.Count > 1)
        {
            return await HandleBatchAsync(
                sender, eto.ChannelId, taskHistory, receiverUsers, eto.MessageData, transmissionContent);
        }

        return await HandleSingleAsync(
            sender, eto.ChannelId, taskHistory, receiverUsers, eto.MessageData, transmissionContent);
    }

    private async Task<List<MessageReceiverUser>> GetReceiverUsersFromDeviceTokensAsync(
    Guid channelId,
    AppPlatform platform,
    ExtraPropertyDictionary variables)
    {
        var deviceTokens = await _appDeviceTokenRepository.GetListAsync(
            x => x.ChannelId == channelId && x.Platform == platform);

        return deviceTokens
            .Select(x => new MessageReceiverUser(
                x.UserId,
                x.DeviceToken,
                variables,
                x.Platform.ToString()))
            .ToList();
    }

    private async Task<MessageSendStatuses> SendMcAppMessageAsync(SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent)
    {
        var messageTaskHistory = eto.MessageTaskHistory;
        var users = messageTaskHistory.ReceiverUsers;
        
        // Separate valid and invalid platform users
        var validUsers = users.Where(user => !string.IsNullOrEmpty(user.Platform) && Enum.TryParse<AppPushProviders>(user.Platform, out _) && !string.IsNullOrEmpty(user.ChannelUserIdentity)).ToList();
        var invalidUsers = users.Where(user => string.IsNullOrEmpty(user.Platform) || !Enum.TryParse<AppPushProviders>(user.Platform, out _) || string.IsNullOrEmpty(user.ChannelUserIdentity)).ToList();

        var sendStatuses = new List<MessageSendStatuses>();

        // Process valid platform users
        if (validUsers.Any())
        {
            var groupByPlatform = GroupUsersByPlatform(validUsers, messageTaskHistory.MessageTask.ReceiverType);
            foreach (var platformGroup in groupByPlatform)
            {
                var result = await SendToPlatformAsync(platformGroup.Key, platformGroup.Value, eto, transmissionContent);
                sendStatuses.Add(result);
            }
        }

        // Process invalid platform users
        if (invalidUsers.Any())
        {
            await ProcessInvalidUsers(invalidUsers, eto, transmissionContent);
            sendStatuses.Add(MessageSendStatuses.Fail);
        }

        return DetermineOverallStatus(sendStatuses);
    }

    private Dictionary<AppPushProviders, IEnumerable<MessageReceiverUser>> GroupUsersByPlatform(IEnumerable<MessageReceiverUser> users, ReceiverTypes receiverType)
    {
        if (receiverType == ReceiverTypes.Broadcast)
        {
            return Enum.GetValues<AppVendor>()
                .Cast<AppVendor>()
                .ToDictionary(
                    vendor => (AppPushProviders)vendor,
                    _ => Enumerable.Empty<MessageReceiverUser>()
                );
        }

        return users
            .Where(user => !string.IsNullOrEmpty(user.Platform) && Enum.TryParse<AppPushProviders>(user.Platform, out _))
            .GroupBy(user => Enum.Parse<AppPushProviders>(user.Platform))
            .ToDictionary(group => group.Key, group => group.AsEnumerable());
    }

    private async Task<MessageSendStatuses> SendToPlatformAsync(AppPushProviders platform, IEnumerable<MessageReceiverUser> users, SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent)
    {
        var vendorConfig = await _appVendorConfigRepository.FindAsync(x => x.ChannelId == eto.ChannelId && x.Vendor == (AppVendor)platform);

        if (vendorConfig == null)
        {
            _logger.LogWarning("No vendor config found for platform: {Platform}", platform);
            return MessageSendStatuses.Fail;
        }

        var options = _appNotificationSenderFactory.GetOptions(platform, vendorConfig.Options);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(platform);

        using (asyncLocal.Change(options))
        {
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(platform);
            return await SendMessageBasedOnReceiverTypeAsync(sender, eto, transmissionContent, eto.MessageTaskHistory, (AppPlatform)platform, users.ToList());
        }
    }

    private async Task ProcessInvalidUsers(IEnumerable<MessageReceiverUser> users, SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent)
    {
        var records = new List<MessageRecord>();

        foreach (var user in users)
        {
            var record = CreateMessageRecord(user, eto.ChannelId, eto.MessageTaskHistory, eto.MessageData);
            if (string.IsNullOrEmpty(user.ChannelUserIdentity))
            {
                record.SetResult(false, "Invalid channel user identity");
            }
            else
            {
                record.SetResult(false, "Invalid platform");
            }
            records.Add(record);
        }

        if (records.Any())
        {
            await _messageRecordRepository.AddRangeAsync(records);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }
    }

    private MessageSendStatuses DetermineOverallStatus(List<MessageSendStatuses> sendStatuses)
    {
        return !sendStatuses.Any(x => x != MessageSendStatuses.Success)
            ? MessageSendStatuses.Success
            : sendStatuses.Any(x => x == MessageSendStatuses.Success)
                ? MessageSendStatuses.PartialFailure
                : MessageSendStatuses.Fail;
    }

    private async Task<MessageSendStatuses> HandleBroadcastAsync(IAppNotificationSender sender, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        try
        {
            var message = CreateAppMessage(data, transmissionContent);
            var response = await sender.BroadcastSendAsync(message);
            return response.Success ? MessageSendStatuses.Success : MessageSendStatuses.Fail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send broadcast message.");
            return MessageSendStatuses.Fail;
        }
    }

    private AppMessage CreateAppMessage(MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        return new AppMessage(
            data.MessageContent.Title,
            data.MessageContent.Content,
            data.GetDataValue<string>(BusinessConsts.INTENT_URL),
            transmissionContent,
            data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
        );
    }

    private async Task<MessageSendStatuses> HandleBatchAsync(
    IAppNotificationSender sender,
    Guid channelId,
    MessageTaskHistory taskHistory,
    List<MessageReceiverUser> receiverUsers,
    MessageData data,
    ExtraPropertyDictionary transmissionContent)
    {
        const int batchSize = 1000;
        var userIdentities = receiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToArray();
        var totalBatches = (int)Math.Ceiling((double)userIdentities.Length / batchSize);
        var successCount = 0;

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = userIdentities.Skip(i * batchSize).Take(batchSize).ToArray();
            var batchRecords = receiverUsers
                .Where(u => batch.Contains(u.ChannelUserIdentity))
                .Select(user => CreateMessageRecord(user, channelId, taskHistory, data))
                .ToList();

            try
            {
                var message = CreateBatchAppMessage(batch, data, transmissionContent);
                var responses = await sender.BatchSendAsync(message);

                successCount += UpdateRecordsWithResponse(batchRecords, batch, responses, sender.SupportsReceipt);

                var isWebsiteMessage = data.GetDataValue<bool>(BusinessConsts.IS_WEBSITE_MESSAGE);
                if (isWebsiteMessage)
                {
                    await AddWebsiteMessages(batchRecords, batch, channelId, data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during BatchSendAsync in HandleBatchAsync");
                foreach (var item in batch)
                {
                    var record = batchRecords.FirstOrDefault(x => x.ChannelUserIdentity == item);
                    if (record is not null)
                    {
                        record.SetResult(false, ex.Message);
                    }
                }
            }

            await _messageRecordRepository.AddRangeAsync(batchRecords);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
        }

        return DetermineBatchStatus(successCount, receiverUsers.Count);
    }


    private BatchAppMessage CreateBatchAppMessage(string[] batch, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        return new BatchAppMessage(
            batch,
            data.MessageContent.Title,
            data.MessageContent.Content,
            data.GetDataValue<string>(BusinessConsts.INTENT_URL),
            transmissionContent,
            data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
        );
    }

    private async Task<MessageSendStatuses> HandleSingleAsync(IAppNotificationSender sender, Guid channelId, MessageTaskHistory taskHistory, List<MessageReceiverUser> receiverUsers, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        var successCount = 0;
        var records = new List<MessageRecord>();
        var websiteMessages = new List<WebsiteMessage>();

        foreach (var user in receiverUsers)
        {
            var record = CreateMessageRecord(user, channelId, taskHistory, data);
            try
            {
                var isWebsiteMessage = data.GetDataValue<bool>(BusinessConsts.IS_WEBSITE_MESSAGE);
                if (isWebsiteMessage)
                {
                    websiteMessages.Add(CreateWebsiteMessage(record, user, data));
                }

                var message = CreateSingleAppMessage(user.ChannelUserIdentity, data, transmissionContent);
                var response = await sender.SendAsync(message);

                UpdateRecordWithResponse(record, response, sender.SupportsReceipt);
                if (response.Success) successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendAppMessageEventHandler");
                record.SetResult(false, ex.Message);
            }

            records.Add(record);
        }

        await _messageRecordRepository.AddRangeAsync(records);
        await _websiteMessageRepository.AddRangeAsync(websiteMessages);

        return DetermineBatchStatus(successCount, receiverUsers.Count);
    }

    private SingleAppMessage CreateSingleAppMessage(string token, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        return new SingleAppMessage(
            token,
            data.MessageContent.Title,
            data.MessageContent.Content,
            data.GetDataValue<string>(BusinessConsts.INTENT_URL),
            transmissionContent,
            data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
        );
    }

    private MessageRecord CreateMessageRecord(MessageReceiverUser user, Guid channelId, MessageTaskHistory taskHistory, MessageData data)
    {
        var record = new MessageRecord(
            user.UserId,
            user.ChannelUserIdentity,
            channelId,
            taskHistory.MessageTaskId,
            taskHistory.Id,
            user.Variables,
            data.MessageContent.Title,
            taskHistory.SendTime,
            taskHistory.MessageTask.SystemId
        );
        record.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
        data.RenderContent(user.Variables);

        if (taskHistory.MessageTask.IsCompensateMessage)
        {
            record.SetCompensate(taskHistory.MessageTask.ExtraProperties);
        }

        return record;
    }

    private WebsiteMessage CreateWebsiteMessage(MessageRecord record, MessageReceiverUser user, MessageData data)
    {
        return new WebsiteMessage(
            record.MessageTaskHistoryId,
            record.ChannelId,
            user.UserId,
            data.MessageContent.Title,
            data.MessageContent.Content,
            data.MessageContent.GetJumpUrl(),
            DateTimeOffset.UtcNow,
            data.MessageContent.ExtraProperties
        );
    }

    private async Task AddWebsiteMessages(List<MessageRecord> batchRecords, string[] batch, Guid channelId, MessageData data)
    {
        var websiteMessages = batchRecords
            .Where(x => batch.Contains(x.ChannelUserIdentity))
            .Select(record =>
                new WebsiteMessage(
                    record.MessageTaskHistoryId,
                    channelId,
                    record.UserId,
                    data.MessageContent.Title,
                    data.MessageContent.Content,
                    data.MessageContent.GetJumpUrl(),
                    DateTimeOffset.UtcNow,
                    data.MessageContent.ExtraProperties
                )).ToList();
        await _websiteMessageRepository.AddRangeAsync(websiteMessages);
    }

    private int UpdateRecordsWithResponse(List<MessageRecord> records, string[] batch, IEnumerable<AppNotificationResponse> responses, bool supportsReceipt)
    {
        var successCount = 0;

        var responseDictionary = responses.ToDictionary(response => response.RegId);

        foreach (var record in records)
        {
            if (responseDictionary.TryGetValue(record.ChannelUserIdentity, out var response))
            {
                if (response.Success)
                {
                    record.SetResult(supportsReceipt == false ? true : null, string.Empty, null, response.MsgId);
                    successCount++;
                }
                else
                {
                    record.SetResult(false, response.Message, null, response.MsgId);
                }
            }
        }

        return successCount;
    }

    private void UpdateRecordWithResponse(MessageRecord record, AppNotificationResponse response, bool supportsReceipt)
    {
        if (response.Success)
        {
            record.SetResult(supportsReceipt == false ? true : null, string.Empty, null, response.MsgId);
        }
        else
        {
            record.SetResult(false, response.Message, null, response.MsgId);
        }
    }

    private MessageSendStatuses DetermineBatchStatus(int successCount, int totalCount)
    {
        return successCount == totalCount
            ? MessageSendStatuses.Success
            : successCount > 0
                ? MessageSendStatuses.PartialFailure
                : MessageSendStatuses.Fail;
    }
}