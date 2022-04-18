namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Aggregates
{
    public class ReceiverGroupUser : Entity<Guid>
    {
        public Guid GroupId { get; protected set; }

        public Guid UserId { get; protected set; }

        public ReceiverGroupUser(Guid groupId, Guid userId)
        {
            GroupId = groupId;
            UserId = userId;
        }
    }
}
