// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetMessageTaskHistoryListQuery(GetMessageTaskHistoryInputDto Input) : Query<PaginatedListDto<MessageTaskHistoryDto>>
{
    public override PaginatedListDto<MessageTaskHistoryDto> Result { get; set; } = default!;

}
