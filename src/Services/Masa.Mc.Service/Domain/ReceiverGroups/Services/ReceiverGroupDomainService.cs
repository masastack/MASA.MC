namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Services;

public class ReceiverGroupDomainService : DomainService
{
    private readonly IReceiverGroupRepository _repository;

    public ReceiverGroupDomainService(IDomainEventBus eventBus, IReceiverGroupRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public virtual async Task SetUsersAsync(ReceiverGroup receiverGroup, params Guid[] userIds)
    {
        foreach (var userId in receiverGroup.Users.Select(x => x.UserId).ToArray())
        {
            if (!userIds.Contains(userId))
            {
                receiverGroup.RemoveUser(userId);
            }
        }
        foreach (var userId in userIds)
        {
            if (!receiverGroup.IsInUser(userId))
            {
                receiverGroup.AddUser(userId);
            }
        }
        await _repository.UpdateAsync(receiverGroup);
    }
}
