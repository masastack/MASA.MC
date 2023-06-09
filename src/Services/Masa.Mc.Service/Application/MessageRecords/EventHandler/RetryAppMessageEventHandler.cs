// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryAppMessageEventHandler
{
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly AppNotificationSenderFactory _appNotificationSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _repository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly II18n<DefaultResource> _i18n;

    public RetryAppMessageEventHandler(IAppNotificationAsyncLocal appNotificationAsyncLocal
        , AppNotificationSenderFactory appNotificationSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository repository
        , IWebsiteMessageRepository websiteMessageRepository
        , IMessageTaskRepository messageTaskRepository
        , II18n<DefaultResource> i18n)
    {
        _appNotificationAsyncLocal = appNotificationAsyncLocal;
        _appNotificationSenderFactory = appNotificationSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _messageTemplateDomainService = messageTemplateDomainService;
        _repository = repository;
        _websiteMessageRepository = websiteMessageRepository;
        _messageTaskRepository = messageTaskRepository;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetryAppMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null) return;
        var channel = await _channelRepository.FindAsync(x => x.Id == messageRecord.ChannelId);
        if (channel == null) return;

        var options = new AppOptions
        {
            AppID = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppID)),
            AppKey = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppKey)),
            AppSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppSecret)),
            MasterSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.MasterSecret))
        };
        using (_appNotificationAsyncLocal.Change(options))
        {
            var messageData = await _taskDomainService.GetMessageDataAsync(messageRecord.MessageTaskId, messageRecord.Variables);

            if (messageData.MessageType == MessageEntityTypes.Template)
            {
                var messageTemplate = await _repository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
                if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                {
                    messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                    await _messageRecordRepository.UpdateAsync(messageRecord);
                    return;
                }
            }

            try
            {
                var appChannel = channel.Type as ChannelType.AppsChannel;
                var transmissionContent = appChannel.GetMessageTransmissionContent(messageData.MessageContent);

                var provider = channel.ExtraProperties.GetProperty<int>(nameof(AppChannelOptions.Provider));
                var appNotificationSender = _appNotificationSenderFactory.GetAppNotificationSender((Providers)provider);
                var response = await appNotificationSender.SendAsync(new SingleAppMessage(messageRecord.ChannelUserIdentity, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.GetDataValue<string>(BusinessConsts.INTENT_URL), transmissionContent, messageData.GetDataValue<bool>(BusinessConsts.IS_APNS_PRODUCTION)));
                if (response.Success)
                {
                    messageRecord.SetResult(true, string.Empty);
                    messageRecord.SetDataValue(BusinessConsts.APP_PUSH_MSG_ID, response.MsgId);
                }
                else
                {
                    messageRecord.SetResult(false, response.Message);
                }
            }
            catch (Exception ex)
            {
                messageRecord.SetResult(false, ex.Message);
            }

            await _messageRecordRepository.UpdateAsync(messageRecord);
        }
    }
}
