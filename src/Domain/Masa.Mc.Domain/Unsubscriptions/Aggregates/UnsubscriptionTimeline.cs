// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Unsubscriptions.Aggregates;

public class UnsubscriptionTimeline : AuditEntity<Guid, Guid>
{
    public Guid UnsubscriptionId { get; private set; }

    public UnsubscriptionTimelineActions Action { get; private set; }

    public UnsubscriptionSource Source { get; private set; }

    public string Detail { get; private set; } = string.Empty;

    public string Keyword { get; private set; } = string.Empty;

    public string MessageId { get; private set; } = string.Empty;

    public Guid? MatchedMessageRecordId { get; private set; }

    public string MatchedMessageSnapshot { get; private set; } = string.Empty;

    public DateTimeOffset? MatchedMessageSentAt { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    private UnsubscriptionTimeline()
    {
    }

    public UnsubscriptionTimeline(
        Guid unsubscriptionId,
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
        UnsubscriptionId = unsubscriptionId;
        Action = action;
        Source = source;
        OccurredAt = occurredAt;
        Detail = detail ?? string.Empty;
        Keyword = keyword ?? string.Empty;
        MessageId = messageId ?? string.Empty;
        MatchedMessageRecordId = matchedMessageRecordId;
        MatchedMessageSnapshot = matchedMessageSnapshot ?? string.Empty;
        MatchedMessageSentAt = matchedMessageSentAt;
    }
}
