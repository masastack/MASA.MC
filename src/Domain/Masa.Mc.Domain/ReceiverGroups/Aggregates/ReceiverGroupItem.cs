// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroupItem : Entity<Guid>
{
    public Guid GroupId { get; protected set; }

    public Receiver Receiver { get; protected set; } = default!;

    public ReceiverGroupItemTypes Type { get; protected set; }

    private ReceiverGroupItem() { }

    public ReceiverGroupItem(Guid groupId, ReceiverGroupItemTypes type, Receiver receiver)
    {
        GroupId = groupId;
        Type = type;
        Receiver= receiver;
    }

    public void SetReceiver(Receiver receiver)
    {
        Receiver = receiver;
    }
}
