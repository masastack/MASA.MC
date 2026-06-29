// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class SmsInboundAutoReplyService : ITransientDependency
{
    private readonly IChannelRepository _channelRepository;
    private readonly ISmsTemplateRepository _smsTemplateRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly ILogger<SmsInboundAutoReplyService> _logger;

    public SmsInboundAutoReplyService(
        IChannelRepository channelRepository,
        ISmsTemplateRepository smsTemplateRepository,
        IMessageRecordRepository messageRecordRepository,
        SmsSenderFactory smsSenderFactory,
        ILogger<SmsInboundAutoReplyService> logger)
    {
        _channelRepository = channelRepository;
        _smsTemplateRepository = smsTemplateRepository;
        _messageRecordRepository = messageRecordRepository;
        _smsSenderFactory = smsSenderFactory;
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

        var request = new SmsInboundAutoReplySendRequest(
            channelId,
            provider,
            channelUserIdentity,
            userId,
            smsTemplate.Id,
            smsTemplate.TemplateCode ?? string.Empty,
            smsTemplate.TemplateName ?? string.Empty,
            smsTemplate.TemplateContent.Trim());

        await TrySendAutoReplyInternalAsync(request, cancellationToken);
    }

    private async Task TrySendAutoReplyInternalAsync(
        SmsInboundAutoReplySendRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var channelId = request.ChannelId;
        var provider = request.Provider;
        var channelUserIdentity = request.ChannelUserIdentity;
        var userId = request.UserId;
        var autoReplyTemplateEntityId = request.AutoReplyTemplateEntityId;
        var autoReplyTemplateId = request.AutoReplyTemplateId;
        var autoReplyTemplateName = request.AutoReplyTemplateName;
        var autoReplyContent = request.AutoReplyContent;

        if (string.IsNullOrWhiteSpace(autoReplyContent) || string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            return;
        }

        var channel = await _channelRepository.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == channelId, cancellationToken);
        if (channel is null)
        {
            return;
        }

        var messageRecord = new MessageRecord(
            userId,
            channelUserIdentity,
            channelId,
            Guid.Empty,
            Guid.Empty,
            new ExtraPropertyDictionary(),
            autoReplyTemplateName ?? string.Empty,
            DateTimeOffset.UtcNow,
            string.Empty);
        messageRecord.SetMessageEntity(MessageEntityTypes.Template, autoReplyTemplateEntityId);
        messageRecord.SetDataValue(nameof(MessageTemplate.TemplateId), autoReplyTemplateId ?? string.Empty);

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
}
