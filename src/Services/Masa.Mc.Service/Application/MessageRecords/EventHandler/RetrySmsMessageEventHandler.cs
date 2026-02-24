// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetrySmsMessageEventHandler
{
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _repository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ITemplateRenderer _templateRenderer;

    public RetrySmsMessageEventHandler(SmsSenderFactory smsSenderFactory
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository repository
        , II18n<DefaultResource> i18n
        , ITemplateRenderer templateRenderer)
    {
        _smsSenderFactory = smsSenderFactory;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _repository = repository;
        _i18n = i18n;
        _templateRenderer = templateRenderer;
    }

    [EventHandler]
    public async Task HandleEventAsync(RetrySmsMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null) return;

        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == messageRecord.ChannelId);
        if (channel == null) return;

        var provider = (SmsProviders)channel.Provider;
        var options = _smsSenderFactory.GetOptions(provider, channel.ExtraProperties);
        var smsAsyncLoca = _smsSenderFactory.GetProviderAsyncLocal(provider);
        var smsSender = _smsSenderFactory.GetSender(provider);

        using (smsAsyncLoca.Change(options))
        {
            var variables = messageRecord.Variables;
            var messageTemplate = await _repository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
            if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
            {
                messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                await _messageRecordRepository.UpdateAsync(messageRecord);
                return;
            }

            variables = _messageTemplateDomainService.ConvertVariables(messageTemplate, messageRecord.Variables);
            var text = BuildSmsMessageText(smsSender, variables, messageTemplate);

            var smsMessage = new SmsMessage(messageRecord.ChannelUserIdentity, text);
            smsMessage.Properties.Add("SignName", messageRecord.GetDataValue<string>(nameof(MessageTemplate.Sign)));
            smsMessage.Properties.Add("TemplateCode", messageRecord.GetDataValue<string>(nameof(MessageTemplate.TemplateId)));
            try
            {
                var response = await smsSender.SendAsync(smsMessage) as SmsSendResponse;
                if (response.Success)
                {
                    messageRecord.SetResult(smsSender.SupportsReceipt ? null : true, string.Empty, DateTimeOffset.UtcNow, response.MsgId);
                }
                else
                {
                    messageRecord.SetResult(false, response.Message, DateTimeOffset.UtcNow, response.MsgId);
                }
            }
            catch (Exception ex)
            {
                messageRecord.SetResult(false, ex.Message);
            }

            await _messageRecordRepository.UpdateAsync(messageRecord);
        }
    }

    private string BuildSmsMessageText(ISmsSender sender, ExtraPropertyDictionary variables, MessageTemplate messageTemplate)
    {
        return sender.SupportsTemplate ? JsonSerializer.Serialize(variables) : _templateRenderer.Render(messageTemplate.MessageContent.Content, variables);
    }
}
