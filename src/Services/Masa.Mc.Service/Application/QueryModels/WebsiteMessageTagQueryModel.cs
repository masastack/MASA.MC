// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class WebsiteMessageTagQueryModel
{
    public Guid Id { get; set; }

    public Guid WebsiteMessageId { get; set; }

    public string Tag { get; set; } = string.Empty;

    public DateTime CreationTime { get; set; }
}
