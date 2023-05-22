// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages.Queries;

public record GetListByTagQuery(string Tags, string ChannelCode) : Query<List<WebsiteMessageTagDto>>
{
    public override List<WebsiteMessageTagDto> Result { get; set; } = default!;
}
