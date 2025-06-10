// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetChannelVendorConfigQuery(Guid ChannelId, AppVendor Vendor) : Query<VendorConfigDto>
{
    public override VendorConfigDto Result { get; set; } = default!;

}
