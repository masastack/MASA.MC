// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Unsubscriptions.Services;

public class UnsubscriptionDomainService : DomainService
{
    private readonly IUnsubscriptionRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public UnsubscriptionDomainService(
        IDomainEventBus eventBus,
        IUnsubscriptionRepository repository,
        II18n<DefaultResource> i18n) : base(eventBus)
    {
        _repository = repository;
        _i18n = i18n;
    }

    public async Task<SmsInboundKeywordAction> HandleSmsInboundKeywordAsync(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        Guid templateId,
        SmsInboundKeywordAction action,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string matchedMessageSnapshot,
        DateTimeOffset? matchedMessageSentAt,
        bool debounceEnabled,
        int cooldownSeconds,
        CancellationToken cancellationToken = default)
    {
        if (action == SmsInboundKeywordAction.Unsubscribe)
        {
            var handled = await HandleInboundUnsubscribeAsync(
                userId,
                channelUserIdentity,
                channelId,
                channelProvider,
                UnsubscriptionScopeTypes.Template,
                templateId.ToString("N"),
                keyword,
                occurredAt,
                inboundMessageId,
                matchedMessageSnapshot,
                matchedMessageSentAt,
                debounceEnabled,
                cooldownSeconds,
                cancellationToken);
            return handled ? SmsInboundKeywordAction.Unsubscribe : SmsInboundKeywordAction.None;
        }

        if (action == SmsInboundKeywordAction.Resubscribe)
        {
            var handled = await HandleInboundResubscribeAsync(
                channelId,
                channelUserIdentity,
                templateId,
                keyword,
                occurredAt,
                inboundMessageId,
                cancellationToken);
            return handled ? SmsInboundKeywordAction.Resubscribe : SmsInboundKeywordAction.None;
        }

        return SmsInboundKeywordAction.None;
    }

    public async Task<bool> HandleSmsInboundProviderReservedUnsubscribeAsync(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string matchedMessageSnapshot,
        DateTimeOffset? matchedMessageSentAt,
        CancellationToken cancellationToken = default)
    {
        return await HandleInboundUnsubscribeAsync(
            userId,
            channelUserIdentity,
            channelId,
            channelProvider,
            UnsubscriptionScopeTypes.Channel,
            string.Empty,
            keyword,
            occurredAt,
            inboundMessageId,
            matchedMessageSnapshot,
            matchedMessageSentAt,
            debounceEnabled: false,
            cooldownSeconds: 0,
            cancellationToken);
    }

    private async Task<bool> HandleInboundUnsubscribeAsync(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        UnsubscriptionScopeTypes scopeType,
        string scopeRefId,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string matchedMessageSnapshot,
        DateTimeOffset? matchedMessageSentAt,
        bool debounceEnabled,
        int cooldownSeconds,
        CancellationToken cancellationToken = default)
    {
        var active = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity,
            scopeType,
            scopeRefId,
            cancellationToken);

        if (active is null)
        {
            var timelineDetail = BuildInboundTimelineDetail(keyword, UnsubscriptionReasonConstants.INBOUND_UNSUBSCRIBE_REASON);
            var outboundMessageDetail = BuildOutboundTimelineDetail(matchedMessageSnapshot);
            var aggregate = Unsubscription.CreateFromSmsInbound(
                userId,
                channelUserIdentity,
                channelId,
                channelProvider,
                scopeType,
                scopeRefId,
                keyword,
                LocalizeReason(UnsubscriptionReasonConstants.INBOUND_UNSUBSCRIBE_REASON),
                occurredAt,
                inboundMessageId,
                timelineDetail,
                outboundMessageDetail,
                matchedMessageSentAt);

            await _repository.AddAsync(aggregate, cancellationToken);
            return true;
        }

        var changed = active.TryUnsubscribeByInboundKeyword(
            keyword,
            LocalizeReason(UnsubscriptionReasonConstants.INBOUND_UNSUBSCRIBE_REASON),
            occurredAt,
            inboundMessageId,
            BuildInboundTimelineDetail(keyword, UnsubscriptionReasonConstants.INBOUND_UNSUBSCRIBE_REASON),
            BuildOutboundTimelineDetail(matchedMessageSnapshot),
            matchedMessageSentAt,
            debounceEnabled,
            cooldownSeconds);
        if (!changed)
        {
            return false;
        }

        await _repository.UpdateAsync(active, cancellationToken);
        return true;
    }

    private async Task<bool> HandleInboundResubscribeAsync(
        Guid channelId,
        string channelUserIdentity,
        Guid templateId,
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        CancellationToken cancellationToken = default)
    {
        var active = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity,
            UnsubscriptionScopeTypes.Template,
            templateId.ToString("N"),
            cancellationToken);
        if (active is null)
        {
            return false;
        }

        active.ResubscribeByInboundKeyword(
            keyword,
            LocalizeReason(UnsubscriptionReasonConstants.INBOUND_RESUBSCRIBE_REASON),
            occurredAt,
            inboundMessageId,
            BuildInboundTimelineDetail(keyword, UnsubscriptionReasonConstants.INBOUND_RESUBSCRIBE_REASON));

        await _repository.UpdateAsync(active, cancellationToken);
        return true;
    }

    public async Task<bool> IsSmsTemplateUnsubscribedAsync(
        Guid channelId,
        string channelUserIdentity,
        Guid templateId,
        CancellationToken cancellationToken = default)
    {
        if (channelId == Guid.Empty || templateId == Guid.Empty || string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            return false;
        }

        var aggregate = await _repository.FindActiveAsync(
            channelId,
            channelUserIdentity.Trim(),
            UnsubscriptionScopeTypes.Template,
            templateId.ToString("N"),
            cancellationToken);

        return aggregate is not null;
    }

    public async Task<bool> IsChannelUserIdentityUnsubscribedAsync(
        string channelUserIdentity,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            return false;
        }

        var existing = await _repository.FindAsync(
            x => x.ChannelUserIdentity == channelUserIdentity.Trim() &&
                 (x.ScopeType == UnsubscriptionScopeTypes.Channel || x.ScopeType == UnsubscriptionScopeTypes.Global) &&
                 x.Status == UnsubscriptionStatus.Unsubscribed,
            cancellationToken);
        return existing is not null;
    }

    public async Task AddChannelUserIdentityToBlacklistAsync(
        Guid userId,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string channelUserIdentity,
        Guid? templateId,
        string reason,
        string operatorName,
        CancellationToken cancellationToken = default)
    {
        if (channelId == Guid.Empty)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_REQUIRED);
        }

        if (string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
        }

        var normalizedChannelUserIdentity = channelUserIdentity.Trim();
        var hasTemplateScope = templateId.HasValue && templateId.Value != Guid.Empty;
        var scopeType = hasTemplateScope ? UnsubscriptionScopeTypes.Template : UnsubscriptionScopeTypes.Channel;
        var scopeRefId = hasTemplateScope ? templateId!.Value.ToString("N") : string.Empty;
        var active = await _repository.FindAsync(
            x => x.ChannelId == channelId &&
                 x.ChannelType == channelType &&
                 x.ChannelProvider == channelProvider &&
                 x.ChannelUserIdentity == normalizedChannelUserIdentity &&
                 (hasTemplateScope
                     ? x.ScopeType == scopeType && x.ScopeRefId == scopeRefId
                     : x.ScopeType == UnsubscriptionScopeTypes.Channel || x.ScopeType == UnsubscriptionScopeTypes.Global) &&
                 x.Status == UnsubscriptionStatus.Unsubscribed,
            cancellationToken);
        if (active is not null)
        {
            return;
        }

        var localizedReason = string.IsNullOrWhiteSpace(reason)
            ? LocalizeReason(UnsubscriptionReasonConstants.MANUAL_BLACKLIST_REASON)
            : reason;
        var timelineDetail = BuildManualUnsubscribedTimelineDetail(operatorName, localizedReason);
        var aggregate = hasTemplateScope
            ? Unsubscription.CreateManualBlacklist(
                userId,
                normalizedChannelUserIdentity,
                channelId,
                channelType,
                channelProvider,
                scopeType,
                scopeRefId,
                localizedReason,
                DateTimeOffset.UtcNow,
                timelineDetail)
            : Unsubscription.CreateChannelManualBlacklist(
                userId,
                normalizedChannelUserIdentity,
                channelId,
                channelType,
                channelProvider,
                localizedReason,
                DateTimeOffset.UtcNow,
                timelineDetail);
        await _repository.AddAsync(aggregate, cancellationToken);
    }

    public async Task RemoveChannelUserIdentityFromBlacklistAsync(
        Guid operatorId,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string channelUserIdentity,
        Guid? templateId,
        string reason,
        string operatorName,
        CancellationToken cancellationToken = default)
    {
        if (channelId == Guid.Empty)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_REQUIRED);
        }

        if (string.IsNullOrWhiteSpace(channelUserIdentity))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
        }

        var normalizedChannelUserIdentity = channelUserIdentity.Trim();
        var hasTemplateScope = templateId.HasValue && templateId.Value != Guid.Empty;
        var scopeType = hasTemplateScope ? UnsubscriptionScopeTypes.Template : UnsubscriptionScopeTypes.Channel;
        var scopeRefId = hasTemplateScope ? templateId!.Value.ToString("N") : string.Empty;
        var active = await _repository.FindAsync(
            x => x.ChannelId == channelId &&
                 x.ChannelType == channelType &&
                 x.ChannelProvider == channelProvider &&
                 x.ChannelUserIdentity == normalizedChannelUserIdentity &&
                 (hasTemplateScope
                     ? x.ScopeType == scopeType && x.ScopeRefId == scopeRefId
                     : x.ScopeType == UnsubscriptionScopeTypes.Channel || x.ScopeType == UnsubscriptionScopeTypes.Global) &&
                 x.Status == UnsubscriptionStatus.Unsubscribed,
            cancellationToken);
        if (active is null)
        {
            return;
        }

        var localizedReason = string.IsNullOrWhiteSpace(reason)
            ? LocalizeReason(UnsubscriptionReasonConstants.MANUAL_REMOVE_BLACKLIST_REASON)
            : reason;
        active.Resubscribe(
            localizedReason,
            DateTimeOffset.UtcNow,
            timelineDetail: BuildManualResubscribedTimelineDetail(operatorName, localizedReason));
        await _repository.UpdateAsync(active, cancellationToken);
    }

    private string LocalizeReason(string reasonKey)
    {
        return _i18n.T(reasonKey);
    }

    private string BuildInboundTimelineDetail(string keyword, string fallbackReasonKey)
    {
        var normalizedKeyword = keyword?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedKeyword))
        {
            return fallbackReasonKey == UnsubscriptionReasonConstants.INBOUND_RESUBSCRIBE_REASON
                ? "收到上行（恢复退订）"
                : "收到上行（退订）";
        }

        return $"收到上行“{normalizedKeyword}”";
    }

    private string BuildOutboundTimelineDetail(string messageSnapshot)
    {
        var normalizedSnapshot = messageSnapshot?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedSnapshot))
        {
            return string.Empty;
        }

        return $"发送了“{normalizedSnapshot}”";
    }

    private string BuildManualUnsubscribedTimelineDetail(string operatorName, string reason)
    {
        return $"{NormalizeOperatorName(operatorName)}手动拉黑/ 原因：{reason?.Trim() ?? string.Empty}";
    }

    private string BuildManualResubscribedTimelineDetail(string operatorName, string reason)
    {
        return $"{NormalizeOperatorName(operatorName)}手动恢复/ 恢复原因： {reason?.Trim() ?? string.Empty}";
    }

    private static string NormalizeOperatorName(string operatorName)
    {
        var normalized = operatorName?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? "-" : normalized;
    }
}
