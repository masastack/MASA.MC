// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class AppMessageTask : Entity<Guid>
{
    public MessageEntityTypes EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }
}
