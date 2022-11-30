// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroup : FullAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public string Description { get; protected set; } = string.Empty;

    public ICollection<ReceiverGroupItem> Items { get; protected set; }

    public ReceiverGroup(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
        Items = new Collection<ReceiverGroupItem>();
    }

    public virtual void AddOrUpdateItem(ReceiverGroupItemTypes type, Receiver receiver)
    {
        var existingItem = Items.SingleOrDefault(item => item.Receiver == receiver && item.Type == type);

        if (existingItem == null)
        {
            Items.Add(new ReceiverGroupItem(Id,
                type,
                receiver));
        }
        else
        {
            existingItem.SetReceiver(receiver);
        }
    }
}
