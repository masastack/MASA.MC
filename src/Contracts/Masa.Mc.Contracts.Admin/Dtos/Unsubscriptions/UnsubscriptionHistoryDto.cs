// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class UnsubscriptionHistoryDto
{
    public Guid Id { get; set; }

    public Guid UnsubscriptionId { get; set; }

    public string UserDisplayName { get; set; } = string.Empty;

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public string ChannelDisplayName { get; set; } = string.Empty;

    public string ScopeDisplayName { get; set; } = string.Empty;

    public UnsubscriptionSource Source { get; set; }

    public UnsubscriptionTimelineActions Action { get; set; }

    public UnsubscriptionStatus Status { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }

    public string OperatorName { get; set; } = string.Empty;
}
