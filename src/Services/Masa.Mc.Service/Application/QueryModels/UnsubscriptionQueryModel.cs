// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class UnsubscriptionQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public ChannelTypes ChannelType { get; set; }

    public Guid ChannelId { get; set; }

    public int ChannelProvider { get; set; }

    public UnsubscriptionScopeTypes ScopeType { get; set; }

    public string ScopeRefId { get; set; } = string.Empty;

    public UnsubscriptionSource Source { get; set; }

    public UnsubscriptionStatus Status { get; set; }

    public string Keyword { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public DateTimeOffset UnsubscribedAt { get; set; }

    public DateTimeOffset? ResubscribedAt { get; set; }

    public string LastInboundMessageId { get; set; } = string.Empty;

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }

    public ICollection<UnsubscriptionTimelineQueryModel> Timelines { get; set; } = new List<UnsubscriptionTimelineQueryModel>();
}
