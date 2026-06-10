// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class UnsubscriptionDto : AuditEntityDto<Guid, Guid>
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public ChannelTypes ChannelType { get; set; }

    public Guid ChannelId { get; set; }

    public string ChannelDisplayName { get; set; } = string.Empty;

    public int ChannelProvider { get; set; }

    public UnsubscriptionScopeTypes ScopeType { get; set; }

    public string ScopeRefId { get; set; } = string.Empty;

    public string ScopeDisplayName { get; set; } = string.Empty;

    public UnsubscriptionSource Source { get; set; }

    public UnsubscriptionStatus Status { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTimeOffset UnsubscribedAt { get; set; }

    public DateTimeOffset? ResubscribedAt { get; set; }

    public string UserDisplayName { get; set; } = string.Empty;

    public string ModifierName { get; set; } = string.Empty;
}
