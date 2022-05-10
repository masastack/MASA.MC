// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels.Queries;

public record GetListChannelQuery(GetChannelInputDto Input) : Query<PaginatedListDto<ChannelDto>>
{
    public override PaginatedListDto<ChannelDto> Result { get; set; } = default!;

}
