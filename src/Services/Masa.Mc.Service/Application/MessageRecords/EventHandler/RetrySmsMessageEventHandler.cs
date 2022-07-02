﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetrySmsMessageEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsSender _smsSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;

    public RetrySmsMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsSender smsSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
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
            var smsMessage = new SmsMessage(messageRecord.GetDataValue<string>(nameof(MessageReceiverUser.PhoneNumber)), JsonSerializer.Serialize(messageRecord.Variables));
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
