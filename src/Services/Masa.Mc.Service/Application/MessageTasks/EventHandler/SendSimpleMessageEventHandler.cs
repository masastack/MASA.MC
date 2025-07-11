// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendSimpleMessageEventHandler
{
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public SendSimpleMessageEventHandler(SmsSenderFactory smsSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository)
    {
        _smsSenderFactory = smsSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSimpleSmsMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Code == eto.ChannelCode);
        var provider = (SmsProviders)channel.Provider;
        var options = _smsSenderFactory.GetOptions(provider, channel.ExtraProperties);
        var smsAsyncLoca = _smsSenderFactory.GetProviderAsyncLocal(provider);
        var smsSender = _smsSenderFactory.GetSender(provider);

        using (smsAsyncLoca.Change(options))
        {
            var sign = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Sign));
            var templateId = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
            var messageEntityId = eto.MessageData.GetDataValue<Guid>(nameof(MessageTemplate.Id));
            var text = smsSender.SupportsTemplate ? JsonSerializer.Serialize(eto.Variables) : eto.MessageData.MessageContent.Content;

            var smsMessage = new SmsMessage(eto.ChannelUserIdentity, text);
            smsMessage.Properties.Add("SignName", sign);
            smsMessage.Properties.Add("TemplateCode", templateId);

            var response = await smsSender.SendAsync(smsMessage);

            var messageRecord = new MessageRecord(Guid.Empty, eto.ChannelUserIdentity, channel.Id, Guid.Empty, Guid.Empty, eto.OriginalVariables, eto.MessageData.MessageContent.Title, DateTimeOffset.UtcNow, eto.SystemId);
            messageRecord.SetMessageEntity(eto.MessageData.MessageType, messageEntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.Sign), sign);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), templateId);

            messageRecord.SetDisplayName(eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));

            if (response?.Success == true)
            {
                messageRecord.SetResult(true, string.Empty, DateTimeOffset.UtcNow, response.MsgId);
            }
            else
            {
                messageRecord.SetResult(false, response?.Message ?? string.Empty, DateTimeOffset.UtcNow, response.MsgId);
            }

            await _messageRecordRepository.AddAsync(messageRecord);
        }
    }
}
