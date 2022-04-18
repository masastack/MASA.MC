namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates;

public class ReceiverGroup : AuditAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public string Description { get; protected set; } = string.Empty;

    public ICollection<ReceiverGroupUser> Users { get; protected set; }

    public ReceiverGroup(string displayName, string description)
    {
        DisplayName = displayName;
        Description = description;
        Users = new Collection<ReceiverGroupUser>();
    }

    public virtual void AddUser(Guid userId)
    {
        if (IsInExecutor(userId))
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
        if (!IsInExecutor(userId))
        {
            return;
        }

        Users.RemoveAll(
            ou => ou.UserId == userId
        );
    }

    public virtual bool IsInExecutor(Guid userId)
    {
        return Users.Any(
            ou => ou.UserId == userId
        );
    }
}
