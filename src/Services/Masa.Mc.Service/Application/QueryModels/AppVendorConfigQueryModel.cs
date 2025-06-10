// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class AppVendorConfigQueryModel : Entity<Guid>, ISoftDelete
{
    public Guid ChannelId { get; set; }

    public AppVendor Vendor { get; set; }

    public ExtraPropertyDictionary Options { get; set; } = new();

    public bool IsDeleted { get; set; }
}
