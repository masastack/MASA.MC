// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class UnsubscriptionTimelineDto
{
    public Guid Id { get; set; }

    public UnsubscriptionTimelineActions Action { get; set; }

    public UnsubscriptionSource Source { get; set; }

    public string Detail { get; set; } = string.Empty;

    public string Keyword { get; set; } = string.Empty;

    public string MessageId { get; set; } = string.Empty;

    public Guid? MatchedMessageRecordId { get; set; }

    public string MatchedMessageSnapshot { get; set; } = string.Empty;

    public DateTimeOffset? MatchedMessageSentAt { get; set; }

    public DateTimeOffset OccurredAt { get; set; }
}
