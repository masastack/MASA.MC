// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroup : FullAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public string Description { get; protected set; } = string.Empty;

    public ICollection<ReceiverGroupUser> Users { get; protected set; }

    public ICollection<ReceiverGroupItem> Items { get; protected set; }

    public ReceiverGroup(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
        Users = new Collection<ReceiverGroupUser>();
        Items = new Collection<ReceiverGroupItem>();
    }

    public virtual void AddUser(Guid userId)
    {
        if (IsInUser(userId))
        {
            return;
        }

        Users.Add(
            new ReceiverGroupUser(
                Id,
                userId
            )
        );
    }

    public virtual void RemoveUser(Guid userId)
    {
        if (!IsInUser(userId))
        {
            return;
        }

        Users.RemoveAll(
            ou => ou.UserId == userId
        );
    }

    public virtual bool IsInUser(Guid userId)
    {
        return Users.Any(
            ou => ou.UserId == userId
        );
    }

    public virtual void AddOrUpdateItem(Guid subjectId, ReceiverGroupItemTypes type, string displayName, string avatar = "", string phoneNumber = "", string email = "")
    {
        var existingItem = Items.SingleOrDefault(item => item.SubjectId == subjectId && item.Type == type);

        if (existingItem == null)
        {
            Items.Add(new ReceiverGroupItem(Id,
                subjectId,
                type,
                displayName,
                avatar,
                phoneNumber,
                email));
        }
        else
        {
            existingItem.SetContent(displayName, avatar, phoneNumber, email);
        }
    }
}
