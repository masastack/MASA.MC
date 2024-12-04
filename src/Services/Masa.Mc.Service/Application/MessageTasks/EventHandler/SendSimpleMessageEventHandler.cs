// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendSimpleMessageEventHandler
{
    private readonly IAliyunSmsAsyncLocal _aliyunSmsAsyncLocal;
    private readonly ISmsSender _smsSender;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public SendSimpleMessageEventHandler(IAliyunSmsAsyncLocal aliyunSmsAsyncLocal
        , ISmsSender smsSender
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository)
    {
        _aliyunSmsAsyncLocal = aliyunSmsAsyncLocal;
        _smsSender = smsSender;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSimpleSmsMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Code == eto.ChannelCode);
        var options = new AliyunSmsOptions
        {
            AccessKeyId = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeyId)),
            AccessKeySecret = channel.GetDataValue<string>(nameof(SmsChannelOptions.AccessKeySecret))
        };
        using (_aliyunSmsAsyncLocal.Change(options))
        {
            var sign = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Sign));
            var templateId = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
            var messageEntityId = eto.MessageData.GetDataValue<Guid>(nameof(MessageTemplate.Id));
            var smsMessage = new SmsMessage(eto.ChannelUserIdentity, JsonSerializer.Serialize(eto.Variables));
            smsMessage.Properties.Add("SignName", sign);
            smsMessage.Properties.Add("TemplateCode", templateId);

            var response = await _smsSender.SendAsync(smsMessage) as SmsSendResponse;

            var messageRecord = new MessageRecord(Guid.Empty, eto.ChannelUserIdentity, channel.Id, Guid.Empty, Guid.Empty, eto.Variables, eto.MessageData.MessageContent.Title, DateTimeOffset.Now, eto.SystemId);
            messageRecord.SetMessageEntity(eto.MessageData.MessageType, messageEntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.Sign), sign);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), templateId);

            messageRecord.SetDisplayName(eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));

            if (response?.Success == true)
            {
                messageRecord.SetResult(true, string.Empty);
            }
            else
            {
                messageRecord.SetResult(false, response?.Message ?? string.Empty);
            }

            await _messageRecordRepository.AddAsync(messageRecord);
        }
    }
}
