// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Unsubscriptions.Aggregates;

public class Unsubscription : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; private set; }

    public string ChannelUserIdentity { get; private set; } = string.Empty;

    public ChannelTypes ChannelType { get; private set; }

    public Guid ChannelId { get; private set; }

    public int ChannelProvider { get; private set; }

    public UnsubscriptionScopeTypes ScopeType { get; private set; }

    public string ScopeRefId { get; private set; } = string.Empty;

    public UnsubscriptionSource Source { get; private set; }

    public UnsubscriptionStatus Status { get; private set; }

    public string Keyword { get; private set; } = string.Empty;

    public string Reason { get; private set; } = string.Empty;

    public DateTimeOffset UnsubscribedAt { get; private set; }

    public DateTimeOffset? ResubscribedAt { get; private set; }

    public string LastInboundMessageId { get; private set; } = string.Empty;

    public ICollection<UnsubscriptionTimeline> Timelines { get; private set; } = new List<UnsubscriptionTimeline>();

    private Unsubscription()
    {
    }

    public static Unsubscription CreateManualBlacklist(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        UnsubscriptionScopeTypes scopeType,
        string scopeRefId,
        string reason,
        DateTimeOffset occurredAt,
        string timelineDetail = "")
    {
        var aggregate = new Unsubscription
        {
            Id = IdGeneratorFactory.SequentialGuidGenerator.NewId(),
            UserId = userId,
            ChannelUserIdentity = channelUserIdentity?.Trim() ?? string.Empty,
            ChannelType = channelType,
            ChannelId = channelId,
            ChannelProvider = channelProvider,
            ScopeType = scopeType,
            ScopeRefId = scopeType is UnsubscriptionScopeTypes.Global or UnsubscriptionScopeTypes.Channel
                ? string.Empty
                : scopeRefId?.Trim() ?? string.Empty,
            Source = UnsubscriptionSource.ManualSupport,
            Status = UnsubscriptionStatus.Unsubscribed,
            Keyword = string.Empty,
            Reason = reason?.Trim() ?? string.Empty,
            UnsubscribedAt = occurredAt,
            LastInboundMessageId = string.Empty
        };

        aggregate.ValidateInvariant();
        aggregate.AppendTimeline(
            UnsubscriptionTimelineActions.ManualUnsubscribed,
            occurredAt,
            string.IsNullOrWhiteSpace(timelineDetail) ? aggregate.Reason : timelineDetail);
        return aggregate;
    }

    public static Unsubscription CreateGlobalManualBlacklist(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string reason,
        DateTimeOffset occurredAt,
        string timelineDetail = "")
    {
        return CreateManualBlacklist(
            userId,
            channelUserIdentity,
            channelId,
            channelType,
            channelProvider,
            UnsubscriptionScopeTypes.Global,
            string.Empty,
            reason,
            occurredAt,
            timelineDetail);
    }

    public static Unsubscription CreateChannelManualBlacklist(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string reason,
        DateTimeOffset occurredAt,
        string timelineDetail = "")
    {
        return CreateManualBlacklist(
            userId,
            channelUserIdentity,
            channelId,
            channelType,
            channelProvider,
            UnsubscriptionScopeTypes.Channel,
            string.Empty,
            reason,
            occurredAt,
            timelineDetail);
    }

    public static Unsubscription CreateFromSmsInbound(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        int channelProvider,
        UnsubscriptionScopeTypes scopeType,
        string scopeRefId,
        string keyword,
        string reason,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string timelineDetail = "",
        string outboundMessageDetail = "",
        DateTimeOffset? outboundMessageSentAt = null)
    {
        var aggregate = new Unsubscription
        {
            Id = IdGeneratorFactory.SequentialGuidGenerator.NewId(),
            UserId = userId,
            ChannelUserIdentity = channelUserIdentity?.Trim() ?? string.Empty,
            ChannelType = ChannelTypes.Sms,
            ChannelId = channelId,
            ChannelProvider = channelProvider,
            ScopeType = scopeType,
            ScopeRefId = scopeRefId?.Trim() ?? string.Empty,
            Source = UnsubscriptionSource.SmsInboundKeyword,
            Status = UnsubscriptionStatus.Unsubscribed,
            Keyword = keyword?.Trim() ?? string.Empty,
            Reason = reason?.Trim() ?? string.Empty,
            UnsubscribedAt = occurredAt,
            LastInboundMessageId = inboundMessageId?.Trim() ?? string.Empty
        };

        aggregate.ValidateInvariant();
        aggregate.AppendOutboundTimelineIfNeeded(outboundMessageDetail, outboundMessageSentAt);
        aggregate.AppendTimeline(
            UnsubscriptionTimelineActions.InboundUnsubscribed,
            occurredAt,
            timelineDetail);
        aggregate.AddDomainEvent(new UnsubscribedBySmsInboundDomainEvent(
            aggregate.Id,
            aggregate.ChannelId,
            aggregate.UserId,
            aggregate.ChannelUserIdentity,
            aggregate.ScopeRefId,
            aggregate.Keyword));

        return aggregate;
    }

    public bool TryUnsubscribeByInboundKeyword(
        string keyword,
        string reason,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string timelineDetail,
        string outboundMessageDetail,
        DateTimeOffset? outboundMessageSentAt)
    {
        if (Status != UnsubscriptionStatus.Unsubscribed)
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.INVALID_UNSUBSCRIPTION_STATUS_TRANSITION);
        }

        var normalizedMessageId = inboundMessageId?.Trim() ?? string.Empty;
        if (IsInboundMessageDuplicated(normalizedMessageId))
        {
            return false;
        }

        return AppendInboundKeyword(
            keyword,
            occurredAt,
            normalizedMessageId,
            reason,
            timelineDetail,
            outboundMessageDetail,
            outboundMessageSentAt);
    }

    public void ResubscribeByInboundKeyword(
        string keyword,
        string reason,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string timelineDetail)
    {
        Resubscribe(
            reason,
            occurredAt,
            UnsubscriptionTimelineActions.AutoResubscribed,
            keyword,
            inboundMessageId,
            timelineDetail);
    }

    private bool AppendInboundKeyword(
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string detail = "",
        string timelineDetail = "",
        string outboundMessageDetail = "",
        DateTimeOffset? outboundMessageSentAt = null)
    {
        if (Status != UnsubscriptionStatus.Unsubscribed)
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.INVALID_UNSUBSCRIPTION_STATUS_TRANSITION);
        }

        var normalizedMessageId = inboundMessageId?.Trim() ?? string.Empty;
        if (!string.IsNullOrEmpty(normalizedMessageId) && string.Equals(LastInboundMessageId, normalizedMessageId, StringComparison.Ordinal))
        {
            return false;
        }

        Keyword = keyword?.Trim() ?? Keyword;
        LastInboundMessageId = normalizedMessageId;
        AppendOutboundTimelineIfNeeded(outboundMessageDetail, outboundMessageSentAt);
        AppendTimeline(
            UnsubscriptionTimelineActions.InboundUnsubscribed,
            occurredAt,
            string.IsNullOrWhiteSpace(timelineDetail) ? detail : timelineDetail);
        return true;
    }

    public void Resubscribe(
        string reason,
        DateTimeOffset occurredAt,
        UnsubscriptionTimelineActions action = UnsubscriptionTimelineActions.ManualResubscribed,
        string keyword = "",
        string inboundMessageId = "",
        string timelineDetail = "")
    {
        if (Status != UnsubscriptionStatus.Unsubscribed)
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.INVALID_UNSUBSCRIPTION_STATUS_TRANSITION);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.UNSUBSCRIPTION_REASON_REQUIRED);
        }

        var normalizedReason = reason.Trim();
        Reason = normalizedReason;
        Status = UnsubscriptionStatus.Resubscribed;
        ResubscribedAt = occurredAt;
        var normalizedMessageId = inboundMessageId?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            Keyword = keyword.Trim();
        }

        if (!string.IsNullOrWhiteSpace(normalizedMessageId))
        {
            LastInboundMessageId = normalizedMessageId;
        }

        AppendTimeline(
            action,
            occurredAt,
            string.IsNullOrWhiteSpace(timelineDetail) ? normalizedReason : timelineDetail);
    }

    private bool IsInboundMessageDuplicated(string inboundMessageId)
    {
        return !string.IsNullOrEmpty(inboundMessageId) &&
               string.Equals(LastInboundMessageId, inboundMessageId, StringComparison.Ordinal);
    }

    private void ValidateInvariant()
    {
        if (string.IsNullOrWhiteSpace(ChannelUserIdentity))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
        }

        if (ScopeType == UnsubscriptionScopeTypes.Template && string.IsNullOrWhiteSpace(ScopeRefId))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.INVALID_UNSUBSCRIPTION_SCOPE_REFERENCE);
        }

        if (string.IsNullOrWhiteSpace(Reason))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.UNSUBSCRIPTION_REASON_REQUIRED);
        }
    }

    private void AppendTimeline(
        UnsubscriptionTimelineActions action,
        DateTimeOffset occurredAt,
        string detail = "")
    {
        Timelines.Add(new UnsubscriptionTimeline(
            Id,
            action,
            occurredAt,
            detail));
    }

    private void AppendOutboundTimelineIfNeeded(string outboundMessageDetail, DateTimeOffset? outboundMessageSentAt)
    {
        if (string.IsNullOrWhiteSpace(outboundMessageDetail) || !outboundMessageSentAt.HasValue)
        {
            return;
        }

        AppendTimeline(
            UnsubscriptionTimelineActions.OutboundMessageSent,
            outboundMessageSentAt.Value,
            outboundMessageDetail);
    }
}
