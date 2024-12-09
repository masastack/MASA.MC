// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Events;

public record ResolveMessageTaskEvent(Guid MessageTaskId, Guid OperatorId = default) : DomainEvent
{
    public MessageTask MessageTask { get; set; } = default!;

    public bool IsStop { get; set; }
}