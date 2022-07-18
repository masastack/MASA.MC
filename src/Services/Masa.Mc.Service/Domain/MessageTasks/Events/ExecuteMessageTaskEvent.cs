// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Events;

public record ExecuteMessageTaskEvent(Guid MessageTaskId, bool IsTest = false, Guid JobId = default, Guid TaskId = default) : DomainEvent
{
}