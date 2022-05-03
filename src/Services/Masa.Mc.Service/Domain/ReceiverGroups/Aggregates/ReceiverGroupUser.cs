// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroupUser : Entity<Guid>
{
    public Guid GroupId { get; protected set; }

    public Guid UserId { get; protected set; }

    protected internal ReceiverGroupUser(Guid groupId, Guid userId)
    {
        GroupId = groupId;
        UserId = userId;
    }
}
