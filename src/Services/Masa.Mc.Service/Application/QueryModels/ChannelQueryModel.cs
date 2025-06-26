// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class ChannelQueryModel : Entity<Guid>, ISoftDelete
{
    public string DisplayName { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelTypes Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsStatic { get; set; }

    public string Scheme { get; set; }

    public string SchemeField { get; set; }

    public int Provider { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    public Guid Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
