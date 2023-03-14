// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class WebsiteMessageQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid ChannelId { get; set; }

    public ChannelQueryModel Channel { get; set; } = default!;

    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string LinkUrl { get; set; } = string.Empty;

    public DateTimeOffset SendTime { get; set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadTime { get; set; }

    public bool IsWithdrawn { get; set; }

    public Guid MessageTaskHistoryId { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    public List<WebsiteMessageTagQueryModel> Tags { get; set; } = new();

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
