// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

internal class UnsubscriptionHistoryItem
{
    public Guid TimelineId { get; set; }

    public Guid UnsubscriptionId { get; set; }

    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public Guid ChannelId { get; set; }

    public UnsubscriptionScopeTypes ScopeType { get; set; }

    public string ScopeRefId { get; set; } = string.Empty;

    public UnsubscriptionSource Source { get; set; }

    public UnsubscriptionTimelineActions Action { get; set; }

    public UnsubscriptionStatus Status { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public Guid Operator { get; set; }
}
