// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendAppMessageEventHandler
{
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly ILogger<SendEmailMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly II18n<DefaultResource> _i18n;

    public SendAppMessageEventHandler(IAppNotificationAsyncLocal appNotificationAsyncLocal
        , AppNotificationSenderFactory appNotificationSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , ILogger<SendEmailMessageEventHandler> logger
        , IMessageTemplateRepository messageTemplateRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , II18n<DefaultResource> i18n)
    {
        _appNotificationAsyncLocal = appNotificationAsyncLocal;
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
        var options = new AppOptions
        {
            AppID = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppID)),
            AppKey = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppKey)),
            AppSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppSecret)),
            MasterSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.MasterSecret))
        };
        using (_appNotificationAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;

            var appChannel = channel.Type as ChannelType.AppsChannel;
            var transmissionContent = appChannel.GetMessageTransmissionContent(eto.MessageData.MessageContent);

            var provider = channel.ExtraProperties.GetProperty<int>(nameof(AppChannelOptions.Provider));
            var appNotificationSender = _appNotificationSenderFactory.GetAppNotificationSender((Providers)provider);

            if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
            {
                await BroadcastAsync(taskHistory, appNotificationSender, eto.MessageData, transmissionContent);
                return;
            }

            if (!taskHistory.MessageTask.Receivers.Any(x => x.Type == MessageTaskReceiverTypes.User))
            {
                await BatchSendAsync(eto.ChannelId, taskHistory, appNotificationSender, eto.MessageData, transmissionContent);
                return;
            }

            await BatchSingleSendAsync(eto.ChannelId, taskHistory, appNotificationSender, eto.MessageData, transmissionContent);
        }
    }

    private async Task BroadcastAsync(MessageTaskHistory taskHistory, IAppNotificationSender appNotificationSender, MessageData messageData, ExtraPropertyDictionary transmissionContent)
    {
        var response = await appNotificationSender.SendAllAsync(new AppMessage(messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, messageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));
        taskHistory.SetResult(response.Success ? MessageTaskHistoryStatuses.Success : MessageTaskHistoryStatuses.Fail);

        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async Task BatchSendAsync(Guid channelId, MessageTaskHistory taskHistory, IAppNotificationSender appNotificationSender, MessageData messageData, ExtraPropertyDictionary transmissionContent)
    {
        var channelUserIdentitys = taskHistory.ReceiverUsers.Select(x => x.ChannelUserIdentity).Distinct().ToArray();
        int limit = 1000;
        var num = channelUserIdentitys.Count() / limit;
        var remainder = channelUserIdentitys.Count() % limit;
        int okCount = 0;
        int totalCount = remainder > 0 ? num + 1 : num;
        for (int i = 0; i < totalCount; i++)
        {
            var clientIds = channelUserIdentitys.Skip(i * limit).Take(limit).ToArray();
            var response = await appNotificationSender.BatchSendAsync(new BatchAppMessage(clientIds, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, messageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));

            if (response.Success)
            {
                okCount++;
            }
        }

        if (taskHistory.MessageTask.IsAppInWebsiteMessage)
        {
            var insertWebsiteMessages = new List<WebsiteMessage>();

            foreach (var item in taskHistory.ReceiverUsers)
            {
                var websiteMessage = new WebsiteMessage(taskHistory.Id, channelId, item.UserId, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now, messageData.MessageContent.ExtraProperties);
                insertWebsiteMessages.Add(websiteMessage);
            }
            await _websiteMessageRepository.AddRangeAsync(insertWebsiteMessages);
        }

        taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));

        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async Task BatchSingleSendAsync(Guid channelId, MessageTaskHistory taskHistory, IAppNotificationSender appNotificationSender, MessageData messageData, ExtraPropertyDictionary transmissionContent)
    {
        int okCount = 0;
        int totalCount = taskHistory.ReceiverUsers.Count;

        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);

        var insertMessageRecords = new List<MessageRecord>();
        var insertWebsiteMessages = new List<WebsiteMessage>();

        foreach (var item in taskHistory.ReceiverUsers)
        {
            var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, messageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageData.RenderContent(item.Variables);

            if (messageData.MessageType == MessageEntityTypes.Template)
            {
                if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                {
                    messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                    insertMessageRecords.Add(messageRecord);
                    continue;
                }
            }

            try
            {
                if (taskHistory.MessageTask.IsAppInWebsiteMessage || messageTemplate?.IsWebsiteMessage == true)
                {
                    var websiteMessage = new WebsiteMessage(messageRecord.MessageTaskHistoryId, messageRecord.ChannelId, item.UserId, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now, messageData.MessageContent.ExtraProperties);
                    insertWebsiteMessages.Add(websiteMessage);
                }

                if (string.IsNullOrEmpty(item.ChannelUserIdentity))
                {
                    messageRecord.SetResult(false, _i18n.T("ClientIdNotBound"));
                }
                else
                {
                    var response = await appNotificationSender.SendAsync(new SingleAppMessage(item.ChannelUserIdentity, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, messageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));

                    if (response.Success)
                    {
                        messageRecord.SetResult(true, string.Empty);
                        messageRecord.SetDataValue(BusinessConsts.APP_PUSH_MSG_ID, response.MsgId);
                        okCount++;
                    }
                    else
                    {
                        messageRecord.SetResult(false, response.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendAppMessageEventHandler");
                messageRecord.SetResult(false, ex.Message);
            }

            insertMessageRecords.Add(messageRecord);
        }

        await _messageRecordRepository.AddRangeAsync(insertMessageRecords);
        await _websiteMessageRepository.AddRangeAsync(insertWebsiteMessages);

        taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));

        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }
}
