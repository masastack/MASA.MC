// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendAppMessageEventHandler
{
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;
    private readonly ITemplateRenderer _templateRenderer;
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
        , ITemplateRenderer templateRenderer
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
        _templateRenderer = templateRenderer;
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
                var response = await appNotificationSender.SendAllAsync(new AppMessage(eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, eto.MessageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));
                taskHistory.SetResult(response.Success ? MessageTaskHistoryStatuses.Success : MessageTaskHistoryStatuses.Fail);

                await _messageTaskHistoryRepository.UpdateAsync(taskHistory);

                return;
            }

            int okCount = 0;
            int totalCount = taskHistory.ReceiverUsers.Count;

            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                eto.MessageData.RenderContent(item.Variables);
                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                try
                {
                    if (taskHistory.MessageTask.IsAppInWebsiteMessage())
                    {
                        var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, item.UserId, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now, eto.MessageData.MessageContent.ExtraProperties);
                        await _websiteMessageRepository.AddAsync(websiteMessage);
                    }

                    var response = await appNotificationSender.SendAsync(new SingleAppMessage(item.ChannelUserIdentity, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, eto.MessageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));
                    if (response.Success)
                    {
                        messageRecord.SetResult(true, string.Empty);
                        okCount++;
                    }
                    else
                    {
                        messageRecord.SetResult(false, response.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendAppMessageEventHandler");
                    messageRecord.SetResult(false, ex.Message);
                }

                await _messageRecordRepository.AddAsync(messageRecord);
            }
            taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));

            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        }
    }
}
