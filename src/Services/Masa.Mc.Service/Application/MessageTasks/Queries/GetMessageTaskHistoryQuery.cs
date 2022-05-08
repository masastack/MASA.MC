// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetMessageTaskHistoryQuery(Guid MessageTaskHistoryId) : Query<MessageTaskHistoryDto>
{
    public override MessageTaskHistoryDto Result { get; set; } = new();
}
