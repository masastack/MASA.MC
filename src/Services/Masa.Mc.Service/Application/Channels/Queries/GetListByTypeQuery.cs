// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetListByTypeQuery(ChannelType Type) : Query<List<ChannelDto>>
{
    public override List<ChannelDto> Result { get; set; } = new();
}
