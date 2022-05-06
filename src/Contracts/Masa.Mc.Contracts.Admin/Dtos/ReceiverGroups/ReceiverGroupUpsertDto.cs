// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class ReceiverGroupUpsertDto
{
    public string DisplayName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<Guid> UserIds { get; set; } = new();

    public List<ReceiverGroupItemDto> Items { get; set; } = new();
}