// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.ReceiverGroups.Aggregates;

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

    public virtual void TryAddItem(ReceiverGroupItem item)
    {
        var existingItem = Items.SingleOrDefault(x => x.SubjectId == item.SubjectId && x.Type == item.Type);

        if (existingItem == null)
        {
            Items.Add(new ReceiverGroupItem(Id, item.Type, item.SubjectId, item.DisplayName, item.Avatar));
        }
    }
}
