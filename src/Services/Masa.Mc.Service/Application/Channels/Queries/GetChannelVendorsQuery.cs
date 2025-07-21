// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetChannelVendorsQuery(Guid ChannelId) : Query<List<AppVendorConfigDto>>
{
    public override List<AppVendorConfigDto> Result { get; set; } = new();
} 