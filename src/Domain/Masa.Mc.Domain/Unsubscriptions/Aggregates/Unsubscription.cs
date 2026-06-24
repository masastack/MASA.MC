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
        DateTimeOffset occurredAt)
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
            UnsubscriptionSource.ManualSupport,
            occurredAt,
            aggregate.Reason);
        return aggregate;
    }

    public static Unsubscription CreateGlobalManualBlacklist(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string reason,
        DateTimeOffset occurredAt)
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
            occurredAt);
    }

    public static Unsubscription CreateChannelManualBlacklist(
        Guid userId,
        string channelUserIdentity,
        Guid channelId,
        ChannelTypes channelType,
        int channelProvider,
        string reason,
        DateTimeOffset occurredAt)
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
            occurredAt);
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
        Guid? matchedMessageRecordId = null,
        string matchedMessageSnapshot = "",
        DateTimeOffset? matchedMessageSentAt = null)
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
        aggregate.AppendTimeline(
            UnsubscriptionTimelineActions.InboundUnsubscribed,
            UnsubscriptionSource.SmsInboundKeyword,
            occurredAt,
            aggregate.Reason,
            aggregate.Keyword,
            aggregate.LastInboundMessageId,
            matchedMessageRecordId,
            matchedMessageSnapshot,
            matchedMessageSentAt);
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
        Guid? matchedMessageRecordId,
        string matchedMessageSnapshot,
        DateTimeOffset? matchedMessageSentAt,
        bool debounceEnabled,
        int cooldownSeconds)
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

        if (IsInInboundDebounceWindow(occurredAt, debounceEnabled, cooldownSeconds))
        {
            return false;
        }

        return AppendInboundKeyword(
            keyword,
            occurredAt,
            normalizedMessageId,
            reason,
            matchedMessageRecordId,
            matchedMessageSnapshot,
            matchedMessageSentAt);
    }

    public void ResubscribeByInboundKeyword(
        string keyword,
        string reason,
        DateTimeOffset occurredAt,
        string inboundMessageId)
    {
        Resubscribe(
            reason,
            occurredAt,
            UnsubscriptionTimelineActions.AutoResubscribed,
            UnsubscriptionSource.SmsInboundKeyword,
            keyword,
            inboundMessageId);
    }

    private bool AppendInboundKeyword(
        string keyword,
        DateTimeOffset occurredAt,
        string inboundMessageId,
        string detail = "",
        Guid? matchedMessageRecordId = null,
        string matchedMessageSnapshot = "",
        DateTimeOffset? matchedMessageSentAt = null)
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
        AppendTimeline(
            UnsubscriptionTimelineActions.InboundUnsubscribed,
            UnsubscriptionSource.SmsInboundKeyword,
            occurredAt,
            detail,
            Keyword,
            LastInboundMessageId,
            matchedMessageRecordId,
            matchedMessageSnapshot,
            matchedMessageSentAt);
        return true;
    }

    public void Resubscribe(
        string reason,
        DateTimeOffset occurredAt,
        UnsubscriptionTimelineActions action = UnsubscriptionTimelineActions.ManualResubscribed,
        UnsubscriptionSource source = UnsubscriptionSource.ManualSupport,
        string keyword = "",
        string inboundMessageId = "")
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
            source,
            occurredAt,
            normalizedReason,
            Keyword,
            LastInboundMessageId);
    }

    private bool IsInboundMessageDuplicated(string inboundMessageId)
    {
        return !string.IsNullOrEmpty(inboundMessageId) &&
               string.Equals(LastInboundMessageId, inboundMessageId, StringComparison.Ordinal);
    }

    private bool IsInInboundDebounceWindow(DateTimeOffset occurredAt, bool debounceEnabled, int cooldownSeconds)
    {
        if (!debounceEnabled || cooldownSeconds <= 0)
        {
            return false;
        }

        var lastInboundUnsubscribeOccurredAt = Timelines
            .Where(x => x.Action == UnsubscriptionTimelineActions.InboundUnsubscribed)
            .OrderByDescending(x => x.OccurredAt)
            .Select(x => x.OccurredAt)
            .FirstOrDefault();
        if (lastInboundUnsubscribeOccurredAt == default)
        {
            return false;
        }

        return occurredAt < lastInboundUnsubscribeOccurredAt.AddSeconds(cooldownSeconds);
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
        UnsubscriptionSource source,
        DateTimeOffset occurredAt,
        string detail = "",
        string keyword = "",
        string messageId = "",
        Guid? matchedMessageRecordId = null,
        string matchedMessageSnapshot = "",
        DateTimeOffset? matchedMessageSentAt = null)
    {
        Timelines.Add(new UnsubscriptionTimeline(
            Id,
            action,
            source,
            occurredAt,
            detail,
            keyword,
            messageId,
            matchedMessageRecordId,
            matchedMessageSnapshot,
            matchedMessageSentAt));
    }
}
