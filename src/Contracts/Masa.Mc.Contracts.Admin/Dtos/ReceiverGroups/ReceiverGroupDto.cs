// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class ReceiverGroupDto : AuditEntityDto<Guid, Guid>
{
    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<ReceiverGroupItemDto> Items { get; set; } = new();

    public string ModifierName { get; set; } = string.Empty;
}