// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendSimpleMessageEventHandler
{
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly UnsubscriptionDomainService _channelUnsubscriptionDomainService;

    public SendSimpleMessageEventHandler(SmsSenderFactory smsSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository messageTemplateRepository
        , II18n<DefaultResource> i18n
        , UnsubscriptionDomainService channelUnsubscriptionDomainService)
    {
        _smsSenderFactory = smsSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _messageTemplateRepository = messageTemplateRepository;
        _i18n = i18n;
        _channelUnsubscriptionDomainService = channelUnsubscriptionDomainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendSimpleSmsMessageEvent eto)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == eto.ChannelCode);
        if (channel == null) return;

        var provider = (SmsProviders)channel.Provider;
        var options = _smsSenderFactory.GetOptions(provider, channel.ExtraProperties);
        var smsAsyncLoca = _smsSenderFactory.GetProviderAsyncLocal(provider);
        var smsSender = _smsSenderFactory.GetSender(provider);

        using (smsAsyncLoca.Change(options))
        {
            var sign = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Sign));
            var templateId = eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.TemplateId));
            var messageEntityId = eto.MessageData.GetDataValue<Guid>(nameof(MessageTemplate.Id));
            MessageTemplate? messageTemplate = null;

            var messageRecord = new MessageRecord(Guid.Empty, eto.ChannelUserIdentity, channel.Id, Guid.Empty, Guid.Empty, eto.Variables, eto.MessageData.MessageContent.Title, DateTimeOffset.UtcNow, eto.SystemId);
            messageRecord.SetMessageEntity(eto.MessageData.MessageType, messageEntityId);
            messageRecord.SetDataValue(nameof(MessageTemplate.Sign), sign);
            messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), templateId);
            messageRecord.SetDisplayName(eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.DisplayName)));

            if (eto.MessageData.MessageType == MessageEntityTypes.Template)
            {
                messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == messageEntityId);
                if (messageTemplate?.GetUnsubscribeConfig().Enabled == true &&
                    await _channelUnsubscriptionDomainService.IsSmsTemplateUnsubscribedAsync(channel.Id, eto.ChannelUserIdentity, messageEntityId))
                {
                    messageRecord.SetResult(false, _i18n.T("MessageBlockedByUnsubscription"));
                    await _messageRecordRepository.AddAsync(messageRecord);
                    return;
                }

                if (messageTemplate != null &&
                    !await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, eto.ChannelUserIdentity))
                {
                    messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                    await _messageRecordRepository.AddAsync(messageRecord);
                    return;
                }
            }

            var text = BuildSmsMessageText(eto, smsSender, messageTemplate);

            var smsMessage = new SmsMessage(eto.ChannelUserIdentity, text);
            smsMessage.Properties.Add("SignName", sign);
            smsMessage.Properties.Add("TemplateCode", templateId);

            try
            {
                var response = await smsSender.SendAsync(smsMessage);

                if (response.Success == true)
                {
                    // 如果支持消息回执，则设置为null等待回执；否则立即标记为成功
                    messageRecord.SetResult(smsSender.SupportsReceipt ? null : true, string.Empty, DateTimeOffset.UtcNow, response.MsgId);
                }
                else
                {
                    messageRecord.SetResult(false, response?.Message ?? string.Empty, DateTimeOffset.UtcNow, response?.MsgId ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                messageRecord.SetResult(false, ex.Message ?? string.Empty, DateTimeOffset.UtcNow, string.Empty);
            }
            
            await _messageRecordRepository.AddAsync(messageRecord);
        }
    }

    private string BuildSmsMessageText(SendSimpleSmsMessageEvent eto, ISmsSender smsSender, MessageTemplate? template)
    {
        string text = string.Empty;
        if (smsSender.SupportsTemplate)
        {
            var variables = template is null ? eto.Variables : _messageTemplateDomainService.ConvertVariables(template, eto.Variables);
            text = JsonSerializer.Serialize(variables);
        }
        else
        {
            text = eto.MessageData.MessageContent.Content;

            if (template?.GetUnsubscribeConfig().Enabled == true)
            {
                text = template.AppendUnsubscribeSuffix(text);
            }
        }

        return text;
    }

}
