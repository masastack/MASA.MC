// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendAppMessageEventHandler
{
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly ILogger<SendEmailMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly II18n<DefaultResource> _i18n;

    public SendAppMessageEventHandler(
        AppNotificationSenderFactory appNotificationSenderFactory,
        IChannelRepository channelRepository,
        IMessageRecordRepository messageRecordRepository,
        IMessageTaskHistoryRepository messageTaskHistoryRepository,
        MessageTemplateDomainService messageTemplateDomainService,
        ILogger<SendEmailMessageEventHandler> logger,
        IMessageTemplateRepository messageTemplateRepository,
        IWebsiteMessageRepository websiteMessageRepository,
        II18n<DefaultResource> i18n)
    {
        _appNotificationSenderFactory = appNotificationSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _logger = logger;
        _messageTemplateRepository = messageTemplateRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendAppMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var provider = (Providers)channel.ExtraProperties.GetProperty<int>(nameof(AppChannelOptions.Provider));
        var options = _appNotificationSenderFactory.GetOptions(provider, channel.ExtraProperties);
        var asyncLocal = _appNotificationSenderFactory.GetProviderAsyncLocal(provider);

        using (asyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            var appChannel = channel.Type as ChannelType.AppsChannel;
            var transmissionContent = appChannel.GetMessageTransmissionContent(eto.MessageData.MessageContent);
            var sender = _appNotificationSenderFactory.GetAppNotificationSender(provider);

            if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
            {
                await HandleBroadcastAsync(taskHistory, sender, eto.MessageData, transmissionContent);
            }
            else if (!taskHistory.MessageTask.Receivers.Any(x => x.Type == MessageTaskReceiverTypes.User))
            {
                await HandleBatchAsync(eto.ChannelId, taskHistory, sender, eto.MessageData, transmissionContent);
            }
            else
            {
                await HandleSingleAsync(eto.ChannelId, taskHistory, sender, eto.MessageData, transmissionContent);
            }
        }
    }

    private async Task HandleBroadcastAsync(MessageTaskHistory taskHistory, IAppNotificationSender sender, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        var response = await sender.BroadcastSendAsync(new AppMessage(
            data.MessageContent.Title,
            data.MessageContent.Content,
            data.GetDataValue<string>(BusinessConsts.INTENT_URL),
            transmissionContent,
            data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
        ));

        taskHistory.SetResult(response.Success ? MessageTaskHistoryStatuses.Success : MessageTaskHistoryStatuses.Fail);
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async Task HandleBatchAsync(Guid channelId, MessageTaskHistory taskHistory, IAppNotificationSender sender, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        var userIdentities = taskHistory.ReceiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToArray();
        const int batchSize = 1000;
        int totalBatches = (int)Math.Ceiling((double)userIdentities.Length / batchSize);
        int successCount = 0;

        for (int i = 0; i < totalBatches; i++)
        {
            var batch = userIdentities.Skip(i * batchSize).Take(batchSize).ToArray();
            var response = await sender.BatchSendAsync(new BatchAppMessage(
                batch,
                data.MessageContent.Title,
                data.MessageContent.Content,
                data.GetDataValue<string>(BusinessConsts.INTENT_URL),
                transmissionContent,
                data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
            ));

            if (response.Success)
                successCount++;
        }

        if (taskHistory.MessageTask.IsAppInWebsiteMessage)
        {
            var websiteMessages = taskHistory.ReceiverUsers.Select(u =>
                new WebsiteMessage(
                    taskHistory.Id,
                    channelId,
                    u.UserId,
                    data.MessageContent.Title,
                    data.MessageContent.Content,
                    data.MessageContent.GetJumpUrl(),
                    DateTimeOffset.UtcNow,
                    data.MessageContent.ExtraProperties
                )).ToList();

            await _websiteMessageRepository.AddRangeAsync(websiteMessages);
        }

        var status = successCount == totalBatches
            ? MessageTaskHistoryStatuses.Success
            : successCount > 0
                ? MessageTaskHistoryStatuses.PartialFailure
                : MessageTaskHistoryStatuses.Fail;

        taskHistory.SetResult(status);
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async Task HandleSingleAsync(Guid channelId, MessageTaskHistory taskHistory, IAppNotificationSender sender, MessageData data, ExtraPropertyDictionary transmissionContent)
    {
        int successCount = 0;
        int totalCount = taskHistory.ReceiverUsers.Count;

        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
        var records = new List<MessageRecord>();
        var websiteMessages = new List<WebsiteMessage>();

        foreach (var user in taskHistory.ReceiverUsers)
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

            if (data.MessageType == MessageEntityTypes.Template &&
                !await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, record.ChannelUserIdentity))
            {
                record.SetResult(false, _i18n.T("DailySendingLimit"));
                records.Add(record);
                continue;
            }

            try
            {
                if (taskHistory.MessageTask.IsAppInWebsiteMessage || messageTemplate?.IsWebsiteMessage == true)
                {
                    websiteMessages.Add(new WebsiteMessage(
                        record.MessageTaskHistoryId,
                        record.ChannelId,
                        user.UserId,
                        data.MessageContent.Title,
                        data.MessageContent.Content,
                        data.MessageContent.GetJumpUrl(),
                        DateTimeOffset.UtcNow,
                        data.MessageContent.ExtraProperties
                    ));
                }

                if (string.IsNullOrEmpty(user.ChannelUserIdentity))
                {
                    record.SetResult(false, _i18n.T("ClientIdNotBound"));
                }
                else
                {
                    var response = await sender.SendAsync(new SingleAppMessage(
                        user.ChannelUserIdentity,
                        data.MessageContent.Title,
                        data.MessageContent.Content,
                        data.GetDataValue<string>(BusinessConsts.INTENT_URL),
                        transmissionContent,
                        data.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)
                    ));

                    if (response.Success)
                    {
                        record.SetResult(true, string.Empty);
                        record.SetDataValue(BusinessConsts.APP_PUSH_MSG_ID, response.MsgId);
                        successCount++;
                    }
                    else
                    {
                        record.SetResult(false, response.Message);
                    }
                }
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

        var finalStatus = successCount == totalCount
            ? MessageTaskHistoryStatuses.Success
            : successCount > 0
                ? MessageTaskHistoryStatuses.PartialFailure
                : MessageTaskHistoryStatuses.Fail;

        taskHistory.SetResult(finalStatus);
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }
}
