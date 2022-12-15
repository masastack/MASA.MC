// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryAppMessageEventHandler
{
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly IAppNotificationSender _appNotificationSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _repository;

    public RetryAppMessageEventHandler(IAppNotificationAsyncLocal appNotificationAsyncLocal
        , IAppNotificationSender appNotificationSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository repository)
    {
        _appNotificationAsyncLocal = appNotificationAsyncLocal;
        _appNotificationSender = appNotificationSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _messageTemplateDomainService = messageTemplateDomainService;
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetryEmailMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null)
        {
            return;
        }
        var channel = await _channelRepository.FindAsync(x => x.Id == messageRecord.ChannelId);
        var options = new GetuiOptions
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
                    messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                    await _messageRecordRepository.UpdateAsync(messageRecord);
                    throw new UserFriendlyException("The maximum number of times to send per day has been reached");
                }
            }

            try
            {
                var appChannel = channel.Type as ChannelType.AppsChannel;
                var transmissionContent = appChannel.GetMessageTransmissionContent(messageData.MessageContent);
                var response = await _appNotificationSender.SendAsync(new AppMessage(messageRecord.ChannelUserIdentity, messageData.MessageContent.Title, messageData.MessageContent.Content, transmissionContent));
                if (response.Success)
                {
                    messageRecord.SetResult(true, string.Empty);
                }
                else
                {
                    messageRecord.SetResult(false, response.Message);
                    throw new UserFriendlyException("Resend message failed");
                }
            }
            catch (Exception ex)
            {
                messageRecord.SetResult(false, ex.Message);
                throw new UserFriendlyException("Resend message failed");
            }

            await _messageRecordRepository.UpdateAsync(messageRecord);
        }
    }
}
