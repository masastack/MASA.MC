// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetListMessageTaskQuery(GetMessageTaskInputDto Input) : Query<PaginatedListDto<MessageTaskDto>>
{
    public override PaginatedListDto<MessageTaskDto> Result { get; set; } = default!;

}
