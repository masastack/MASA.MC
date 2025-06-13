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

    public SendAppMessageEventHandler(
        AppNotificationSenderFactory appNotificationSenderFactory,
        IChannelRepository channelRepository,
        IMessageRecordRepository messageRecordRepository,
        IMessageTaskHistoryRepository messageTaskHistoryRepository,
        ILogger<SendAppMessageEventHandler> logger,
        IWebsiteMessageRepository websiteMessageRepository,
        IAppVendorConfigRepository appVendorConfigRepository)
    {
        _appNotificationSenderFactory = appNotificationSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _logger = logger;
        _websiteMessageRepository = websiteMessageRepository;
        _appVendorConfigRepository = appVendorConfigRepository;
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

        var channelProvider = channel.ExtraProperties.GetProperty<int>(nameof(AppChannelOptions.Provider));

        if (channelProvider == (int)AppChannelProviders.Mc)
        {
            var sendStatus = await SendMcAppMessageAsync(eto, transmissionContent);
            await SetTaskHistoryStatusAsync(taskHistory, sendStatus);
        }
        else
        {
            var sendStatus = await SendThirdPartyAppMessageAsync(eto, channelProvider, channel.ExtraProperties, transmissionContent, taskHistory);
            await SetTaskHistoryStatusAsync(taskHistory, sendStatus);
        }
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
        var appSenderProvider = (Providers)channelProvider;
        var options = _appNotificationSenderFactory.GetOptions(appSenderProvider, channelProperties);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(appSenderProvider);

        using (asyncLocal.Change(options))
        {
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(appSenderProvider);
            return await SendMessageBasedOnReceiverTypeAsync(sender, eto, transmissionContent, taskHistory);
        }
    }

    private async Task<MessageSendStatuses> SendMessageBasedOnReceiverTypeAsync(IAppNotificationSender sender, SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent, MessageTaskHistory taskHistory)
    {
        var receiverType = taskHistory.MessageTask.ReceiverType;
        var isUniformContent = taskHistory.MessageTask.IsUniformContent;
        var isWebsiteMessage = taskHistory.MessageTask.IsAppInWebsiteMessage;

        return receiverType switch
        {
            ReceiverTypes.Broadcast => await HandleBroadcastAsync(sender, eto.MessageData, transmissionContent),
            _ when isUniformContent => await HandleBatchAsync(sender, eto.ChannelId, taskHistory, taskHistory.ReceiverUsers, eto.MessageData, transmissionContent, isWebsiteMessage),
            _ => await HandleSingleAsync(sender, eto.ChannelId, taskHistory, taskHistory.ReceiverUsers, eto.MessageData, transmissionContent)
        };
    }

    private async Task<MessageSendStatuses> SendMcAppMessageAsync(SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent)
    {
        var messageData = eto.MessageData;
        var messageTaskHistory = eto.MessageTaskHistory;
        var groupByPlatform = GroupUsersByPlatform(messageTaskHistory.ReceiverUsers);

        var sendStatuses = new List<MessageSendStatuses>();

        foreach (var platformGroup in groupByPlatform)
        {
            var result = await SendToPlatformAsync(platformGroup.Key, platformGroup.Value, eto, transmissionContent);
            sendStatuses.Add(result);
        }

        return DetermineOverallStatus(sendStatuses);
    }

    private Dictionary<Providers, IEnumerable<MessageReceiverUser>> GroupUsersByPlatform(IEnumerable<MessageReceiverUser> users)
    {
        return users
            .Where(user => Enum.TryParse<Providers>(user.Platform, out _))
            .GroupBy(user => Enum.Parse<Providers>(user.Platform))
            .ToDictionary(group => group.Key, group => group.AsEnumerable());
    }

    private async Task<MessageSendStatuses> SendToPlatformAsync(Providers platform, IEnumerable<MessageReceiverUser> users, SendAppMessageEvent eto, ExtraPropertyDictionary transmissionContent)
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
            return await SendMessageBasedOnReceiverTypeAsync(sender, eto, transmissionContent, eto.MessageTaskHistory);
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

    private async Task<MessageSendStatuses> HandleBatchAsync(IAppNotificationSender sender, Guid channelId, MessageTaskHistory taskHistory, List<MessageReceiverUser> receiverUsers, MessageData data, ExtraPropertyDictionary transmissionContent, bool isWebsiteMessage)
    {
        const int batchSize = 1000;
        var userIdentities = receiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToArray();
        var totalBatches = (int)Math.Ceiling((double)userIdentities.Length / batchSize);
        var successCount = 0;
        var records = receiverUsers.Select(user => CreateMessageRecord(user, channelId, taskHistory, data)).ToList();

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = userIdentities.Skip(i * batchSize).Take(batchSize).ToArray();
            try
            {
                var message = CreateBatchAppMessage(batch, data, transmissionContent);
                var response = await sender.BatchSendAsync(message);

                successCount += UpdateRecordsWithResponse(records, batch, response);
                if (isWebsiteMessage)
                {
                    await AddWebsiteMessages(records, batch, channelId, data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during BatchSendAsync in HandleBatchAsync");
                foreach (var item in batch)
                {
                    var record = records.FirstOrDefault(x => x.ChannelUserIdentity == item);
                    if (record is not null)
                    {
                        record.SetResult(false, ex.Message);
                    }
                }
            }
        }

        await _messageRecordRepository.AddRangeAsync(records);
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
                if (taskHistory.MessageTask.IsAppInWebsiteMessage)
                {
                    websiteMessages.Add(CreateWebsiteMessage(record, user, data));
                }

                var message = CreateSingleAppMessage(user.ChannelUserIdentity, data, transmissionContent);
                var response = await sender.SendAsync(message);

                UpdateRecordWithResponse(record, response);
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

    private int UpdateRecordsWithResponse(List<MessageRecord> records, string[] batch, AppNotificationResponse response)
    {
        var successCount = 0;
        foreach (var item in batch)
        {
            var record = records.FirstOrDefault(x => x.ChannelUserIdentity == item);
            if (record is not null)
            {
                if (!response.Success || response.ErrorTokens.Contains(item))
                {
                    record.SetResult(false, response.Message);
                }
                else
                {
                    record.SetResult(true, string.Empty);
                    record.SetDataValue(BusinessConsts.APP_PUSH_MSG_ID, response.MsgId);
                    successCount++;
                }
            }
        }
        return successCount;
    }

    private async Task AddWebsiteMessages(List<MessageRecord> records, string[] batch, Guid channelId, MessageData data)
    {
        var websiteMessages = records.Where(x => batch.Contains(x.ChannelUserIdentity)).Select(record =>
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

    private void UpdateRecordWithResponse(MessageRecord record, AppNotificationResponse response)
    {
        if (response.Success)
        {
            record.SetResult(true, string.Empty);
            record.SetDataValue(BusinessConsts.APP_PUSH_MSG_ID, response.MsgId);
        }
        else
        {
            record.SetResult(false, response.Message);
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