namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Services;

public class ReceiverGroupDomainService : DomainService
{
    private readonly IReceiverGroupRepository _repository;

    public ReceiverGroupDomainService(IDomainEventBus eventBus, IReceiverGroupRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public virtual void SetUsers(ReceiverGroup receiverGroup, params Guid[] userIds)
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
    }

    public virtual async Task CreateAsync(ReceiverGroup receiverGroup, params Guid[] userIds)
    {
        if (userIds != null)
        {
            SetUsers(receiverGroup, userIds);
        }
        await _repository.AddAsync(receiverGroup);
    }

    public virtual async Task UpdateAsync(ReceiverGroup receiverGroup, params Guid[] userIds)
    {
        if (userIds != null)
        {
            SetUsers(receiverGroup, userIds);
        }
        await _repository.UpdateAsync(receiverGroup);
    }
}
