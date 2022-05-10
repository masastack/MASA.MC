// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Queries;

public record GetMessageTaskQuery(Guid MessageTaskId) : Query<MessageTaskDto>
{
    public override MessageTaskDto Result { get; set; } = new();
}
