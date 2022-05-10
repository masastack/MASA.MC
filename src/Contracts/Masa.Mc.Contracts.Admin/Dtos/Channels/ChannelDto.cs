// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels;

public class ChannelDto : AuditEntityDto<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ChannelTypes Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsStatic { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; set; } = new();

    public string ModifierName { get; set; } = string.Empty;
}
