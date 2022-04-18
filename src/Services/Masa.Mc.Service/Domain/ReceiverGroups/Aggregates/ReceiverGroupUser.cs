namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates
{
    public class ReceiverGroupUser : Entity<Guid>
    {
        public virtual Guid GroupId { get; protected set; }

        public virtual Guid UserId { get; protected set; }

        protected internal ReceiverGroupUser(Guid groupId, Guid userId)
        {
            GroupId = groupId;
            UserId = userId;
        }
    }
}
