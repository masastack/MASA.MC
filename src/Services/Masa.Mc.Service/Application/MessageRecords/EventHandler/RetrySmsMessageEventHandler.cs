// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetrySmsMessageEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsSender _smsSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;

    public RetrySmsMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsSender smsSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _messageTemplateDomainService = messageTemplateDomainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetrySmsMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null)
        {
            return;
        }
        var channel = await _channelRepository.FindAsync(x => x.Id == messageRecord.ChannelId);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var messageData = await _taskDomainService.GetMessageDataAsync(messageRecord.MessageTaskId);

            if (messageData.MessageType == MessageEntityTypes.Template)
            {
                var perDayLimit = messageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageRecord.MessageEntityId, perDayLimit, messageRecord.UserId))
                {
                    messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                    await _messageRecordRepository.UpdateAsync(messageRecord);
                    throw new UserFriendlyException("The maximum number of times to send per day has been reached");
                }
            }

            var variables = _messageTemplateDomainService.ConvertVariables(messageData.TemplateItems, messageRecord.Variables);
            var smsMessage = new SmsMessage(messageRecord.GetDataValue<string>(nameof(MessageReceiverUser.PhoneNumber)), JsonSerializer.Serialize(variables));
            smsMessage.Properties.Add("SignName", messageData.GetDataValue<string>(nameof(MessageTemplate.Sign)));
            smsMessage.Properties.Add("TemplateCode", messageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));
            try
            {
                var response = await _smsSender.SendAsync(smsMessage) as SmsSendResponse;
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
