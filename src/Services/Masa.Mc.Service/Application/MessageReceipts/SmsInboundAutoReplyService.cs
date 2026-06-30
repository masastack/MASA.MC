// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class SmsInboundAutoReplyService : ITransientDependency
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly II18n<DefaultResource> _i18n;
    private readonly ILogger<SmsInboundAutoReplyService> _logger;

    public SmsInboundAutoReplyService(
        IChannelRepository channelRepository,
        IMessageTemplateRepository messageTemplateRepository,
        IMessageRecordRepository messageRecordRepository,
        MessageTemplateDomainService messageTemplateDomainService,
        SmsSenderFactory smsSenderFactory,
        IHostEnvironment hostEnvironment,
        II18n<DefaultResource> i18n,
        ILogger<SmsInboundAutoReplyService> logger)
    {
        _channelRepository = channelRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _smsSenderFactory = smsSenderFactory;
        _hostEnvironment = hostEnvironment;
        _i18n = i18n;
        _logger = logger;
    }

    public async Task TrySendAutoReplyAsync(
        Guid channelId,
        SmsInboundProviders provider,
        string channelUserIdentity,
        Guid userId,
        Guid autoReplyTemplateId,
        CancellationToken cancellationToken = default)
    {
        if (autoReplyTemplateId == Guid.Empty)
        {
            return;
        }

        var messageTemplate = await _messageTemplateRepository.FindAsync(
            x => x.ChannelId == channelId && x.Id == autoReplyTemplateId,
            cancellationToken: cancellationToken);
        if (messageTemplate is null)
        {
            return;
        }

        var autoReplyContent = messageTemplate?.MessageContent?.Content?.Trim();
        if (string.IsNullOrWhiteSpace(autoReplyContent))
        {
            return;
        }

        var request = new SmsInboundAutoReplySendRequest(
            channelId,
            provider,
            channelUserIdentity,
            userId,
            messageTemplate.Id,
            messageTemplate.TemplateId,
            messageTemplate.DisplayName,
            autoReplyContent);

        await TrySendAutoReplyInternalAsync(request, messageTemplate, cancellationToken);
    }

    private async Task TrySendAutoReplyInternalAsync(
        SmsInboundAutoReplySendRequest request,
        MessageTemplate? messageTemplate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!TryGetSendPayload(request, out var channelUserIdentity, out var autoReplyContent))
        {
            return;
        }

        var messageRecord = CreateMessageRecord(request);
        if (await TryHandleSendUpperLimitAsync(messageTemplate, channelUserIdentity, messageRecord))
        {
            return;
        }

        var channelId = request.ChannelId;
        var provider = request.Provider;
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Id == channelId, cancellationToken);
        if (channel is null)
        {
            return;
        }

        if (await TryHandleNonProductionAsync(provider, channelId, channelUserIdentity, messageRecord))
        {
            return;
        }

        await SendAutoReplyInProductionAsync(channel, provider, channelUserIdentity, autoReplyContent, messageRecord);
    }

    private static bool TryGetSendPayload(
        SmsInboundAutoReplySendRequest request,
        out string channelUserIdentity,
        out string autoReplyContent)
    {
        channelUserIdentity = request.ChannelUserIdentity;
        autoReplyContent = request.AutoReplyContent;
        return !string.IsNullOrWhiteSpace(autoReplyContent) && !string.IsNullOrWhiteSpace(channelUserIdentity);
    }

    private async Task<bool> TryHandleSendUpperLimitAsync(
        MessageTemplate? messageTemplate,
        string channelUserIdentity,
        MessageRecord messageRecord)
    {
        if (messageTemplate == null ||
            await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, channelUserIdentity))
        {
            return false;
        }

        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"), DateTimeOffset.UtcNow, string.Empty);
        await _messageRecordRepository.AddAsync(messageRecord);
        return true;
    }

    private async Task<bool> TryHandleNonProductionAsync(
        SmsInboundProviders provider,
        Guid channelId,
        string channelUserIdentity,
        MessageRecord messageRecord)
    {
        if (_hostEnvironment.IsProduction())
        {
            return false;
        }

        messageRecord.SetResult(true, _i18n.T("InboundAutoReplySkippedInNonProduction"), DateTimeOffset.UtcNow, string.Empty);
        _logger.LogInformation(
            "Skip inbound auto reply real send in non-production. Provider: {Provider}, ChannelId: {ChannelId}, Target: {Target}",
            provider,
            channelId,
            channelUserIdentity);
        await _messageRecordRepository.AddAsync(messageRecord);
        return true;
    }

    private async Task SendAutoReplyInProductionAsync(
        Channel channel,
        SmsInboundProviders provider,
        string channelUserIdentity,
        string autoReplyContent,
        MessageRecord messageRecord)
    {
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
