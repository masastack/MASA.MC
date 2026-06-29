// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class SmsInboundAutoReplyService : ITransientDependency
{
    private readonly IChannelRepository _channelRepository;
    private readonly ISmsTemplateRepository _smsTemplateRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ILogger<SmsInboundAutoReplyService> _logger;

    public SmsInboundAutoReplyService(
        IChannelRepository channelRepository,
        ISmsTemplateRepository smsTemplateRepository,
        IMessageTemplateRepository messageTemplateRepository,
        IMessageRecordRepository messageRecordRepository,
        MessageTemplateDomainService messageTemplateDomainService,
        SmsSenderFactory smsSenderFactory,
        II18n<DefaultResource> i18n,
        ILogger<SmsInboundAutoReplyService> logger)
    {
        _channelRepository = channelRepository;
        _smsTemplateRepository = smsTemplateRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _smsSenderFactory = smsSenderFactory;
        _i18n = i18n;
        _logger = logger;
    }

    public async Task TrySendAutoReplyAsync(
        Guid channelId,
        SmsInboundProviders provider,
        string channelUserIdentity,
        Guid userId,
        string autoReplyTemplateId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(autoReplyTemplateId))
        {
            return;
        }

        var smsTemplate = await _smsTemplateRepository.FindAsync(
            x => x.ChannelId == channelId && x.TemplateCode == autoReplyTemplateId,
            cancellationToken);
        if (smsTemplate is null || string.IsNullOrWhiteSpace(smsTemplate.TemplateContent))
        {
            return;
        }

        var messageTemplate = await _messageTemplateRepository.FindAsync(
            x => x.ChannelId == channelId && x.TemplateId == autoReplyTemplateId,
            cancellationToken: cancellationToken);

        var request = new SmsInboundAutoReplySendRequest(
            channelId,
            provider,
            channelUserIdentity,
            userId,
            messageTemplate?.Id ?? smsTemplate.Id,
            smsTemplate.TemplateCode ?? string.Empty,
            messageTemplate?.DisplayName ?? smsTemplate.TemplateName ?? string.Empty,
            smsTemplate.TemplateContent.Trim());

        await TrySendAutoReplyInternalAsync(request, messageTemplate, cancellationToken);
    }

    private async Task TrySendAutoReplyInternalAsync(
        SmsInboundAutoReplySendRequest request,
        MessageTemplate? messageTemplate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var channelId = request.ChannelId;
        var provider = request.Provider;
        var channelUserIdentity = request.ChannelUserIdentity;
        var autoReplyContent = request.AutoReplyContent;

        if (string.IsNullOrWhiteSpace(autoReplyContent) || string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            return;
        }

        var messageRecord = CreateMessageRecord(request);
        if (messageTemplate != null &&
            !await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, channelUserIdentity))
        {
            messageRecord.SetResult(false, _i18n.T("DailySendingLimit"), DateTimeOffset.UtcNow, string.Empty);
            await _messageRecordRepository.AddAsync(messageRecord);
            return;
        }

        var channel = await _channelRepository.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == channelId, cancellationToken);
        if (channel is null)
        {
            return;
        }

        var smsProvider = (SmsProviders)provider;
        var options = _smsSenderFactory.GetOptions(smsProvider, channel.ExtraProperties);
        var smsAsyncLocal = _smsSenderFactory.GetProviderAsyncLocal(smsProvider);
        var smsSender = _smsSenderFactory.GetSender(smsProvider);

        if (smsSender.SupportsTemplate)
        {
            _logger.LogWarning("Skip inbound auto reply for provider {Provider} because template mode is required", provider);
            return;
        }

        try
        {
            using (smsAsyncLocal.Change(options))
            {
                var response = await smsSender.SendAsync(new SmsMessage(channelUserIdentity, autoReplyContent.Trim()));
                if (!response.Success)
                {
                    _logger.LogWarning("Inbound auto reply failed: {Message}", response.Message);
                }

                messageRecord.SetResult(
                    response.Success ? (smsSender.SupportsReceipt ? null : true) : false,
                    response.Success ? string.Empty : (response.Message ?? string.Empty),
                    DateTimeOffset.UtcNow,
                    response.MsgId ?? string.Empty);
            }
        }
        catch (Exception ex)
        {
            messageRecord.SetResult(false, ex.Message, DateTimeOffset.UtcNow, string.Empty);
            _logger.LogError(ex, "Send inbound auto reply failed");
        }

        await _messageRecordRepository.AddAsync(messageRecord);
    }

    private static MessageRecord CreateMessageRecord(SmsInboundAutoReplySendRequest request)
    {
        var messageRecord = new MessageRecord(
            request.UserId,
            request.ChannelUserIdentity,
            request.ChannelId,
            Guid.Empty,
            Guid.Empty,
            new ExtraPropertyDictionary(),
            request.AutoReplyTemplateName ?? string.Empty,
            DateTimeOffset.UtcNow,
            string.Empty);
        messageRecord.SetMessageEntity(MessageEntityTypes.Template, request.AutoReplyTemplateEntityId);
        messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), request.AutoReplyTemplateId ?? string.Empty);
        return messageRecord;
    }
}
