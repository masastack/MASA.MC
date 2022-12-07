// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class MessageTemplateItemQueryModel : Entity<Guid>
{
    public Guid MessageTemplateId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string MappingCode { get; set; } = string.Empty;

    public string DisplayText { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
