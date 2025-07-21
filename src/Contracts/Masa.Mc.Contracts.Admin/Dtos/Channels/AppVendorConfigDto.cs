// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels;

public class AppVendorConfigDto
{
    public Guid Id { get; set; }
    public AppVendor Vendor { get; set; }
    public ExtraPropertyDictionary Options { get; set; } = new();
}