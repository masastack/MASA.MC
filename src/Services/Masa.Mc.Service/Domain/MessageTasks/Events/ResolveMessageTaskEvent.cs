// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Events;

public record ResolveMessageTaskEvent(Guid MessageTaskId) : DomainEvent
{
    public MessageTask MessageTask { get; set; }
}