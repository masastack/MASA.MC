// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts;

public class SmsInboundAutoReplyService : ITransientDependency
{
    private readonly IChannelRepository _channelRepository;
    private readonly SmsSenderFactory _smsSenderFactory;
    private readonly ILogger<SmsInboundAutoReplyService> _logger;

    public SmsInboundAutoReplyService(
        IChannelRepository channelRepository,
        SmsSenderFactory smsSenderFactory,
        ILogger<SmsInboundAutoReplyService> logger)
    {
        _channelRepository = channelRepository;
        _smsSenderFactory = smsSenderFactory;
        _logger = logger;
    }

    public async Task TrySendAutoReplyAsync(
        Guid channelId,
        SmsInboundProviders provider,
        string channelUserIdentity,
        string autoReplyContent,
        CancellationToken cancellationToken = default)
    {
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
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send inbound auto reply failed");
        }
    }
}
