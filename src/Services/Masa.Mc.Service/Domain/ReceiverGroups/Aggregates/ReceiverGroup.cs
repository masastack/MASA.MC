namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroup : AuditAggregateRoot<Guid, Guid>
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

    public virtual void AddOrUpdateItem(string dataId, ReceiverGroupItemType type, string displayName, string avatar = "", string phoneNumber = "", string email = "")
    {
        var existingItem = Items.SingleOrDefault(item => item.DataId == dataId && item.Type == type);

        if (existingItem == null)
        {
            Items.Add(new ReceiverGroupItem(Id,
                dataId,
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
