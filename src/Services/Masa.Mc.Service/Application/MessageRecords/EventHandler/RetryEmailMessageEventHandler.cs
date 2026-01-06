// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryEmailMessageEventHandler
{
    private readonly IEmailAsyncLocal _emailAsyncLocal;
    private readonly IEmailSender _emailSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public RetryEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal
        , IEmailSender emailSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository repository
        , II18n<DefaultResource> i18n)
    {
        _emailAsyncLocal = emailAsyncLocal;
        _emailSender = emailSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _messageTemplateDomainService = messageTemplateDomainService;
        _repository = repository;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetryEmailMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null) return;

        var channel = await _channelRepository.AsNoTracking().FirstAsync(x => x.Id == messageRecord.ChannelId);
        if (channel == null) return;

        var options = new SmtpEmailOptions
        {
            Host = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Smtp)),
            Port = channel.ExtraProperties.GetProperty<int>(nameof(EmailChannelOptions.Port)),
            UserName = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName)),
            Password = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.Password)),
            EnableSsl = channel.ExtraProperties.GetProperty<bool>(nameof(EmailChannelOptions.Ssl)),
            DefaultFromAddress = channel.ExtraProperties.GetProperty<string>(nameof(EmailChannelOptions.UserName))
        };
        using (_emailAsyncLocal.Change(options))
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
                await _emailSender.SendAsync(
                        messageRecord.ChannelUserIdentity,
                        messageData.MessageContent.Title,
                        messageData.MessageContent.Content
                    );
                messageRecord.SetResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                messageRecord.SetResult(false, ex.Message);
            }

            await _messageRecordRepository.UpdateAsync(messageRecord);
        }
    }
}
