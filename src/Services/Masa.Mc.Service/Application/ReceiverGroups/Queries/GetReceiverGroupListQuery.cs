// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;

public record GetReceiverGroupListQuery(GetReceiverGroupInputDto Input) : Query<PaginatedListDto<ReceiverGroupDto>>
{
    public override PaginatedListDto<ReceiverGroupDto> Result { get; set; } = default!;

}