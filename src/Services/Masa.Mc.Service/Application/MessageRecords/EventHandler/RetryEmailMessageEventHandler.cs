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

    public RetryEmailMessageEventHandler(IEmailAsyncLocal emailAsyncLocal
        , IEmailSender emailSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _emailAsyncLocal = emailAsyncLocal;
        _emailSender = emailSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _messageTemplateDomainService = messageTemplateDomainService;
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
            var messageData = await _taskDomainService.GetMessageDataAsync(messageRecord.MessageTaskId);

            if (messageData.MessageType == MessageEntityTypes.Template)
            {
                var perDayLimit = messageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageRecord.MessageEntityId, perDayLimit, messageRecord.UserId.ToString()))
                {
                    messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                    await _messageRecordRepository.UpdateAsync(messageRecord);
                    throw new UserFriendlyException("The maximum number of times to send per day has been reached");
                }
            }

            try
            {
                await _emailSender.SendAsync(
                        messageRecord.GetDataValue<string>(nameof(Receiver.Email)),
                        messageData.GetDataValue<string>(nameof(MessageContent.Title)),
                        messageData.GetDataValue<string>(nameof(MessageContent.Content))
                    );
                messageRecord.SetResult(true, string.Empty);
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
