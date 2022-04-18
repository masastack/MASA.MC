namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupCommandHandler
{
    private readonly IReceiverGroupRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly ReceiverGroupDomainService _domainService;

    public ReceiverGroupCommandHandler(IReceiverGroupRepository repository, IIntegrationEventBus integrationEventBus, ReceiverGroupDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task CreateAsync(CreateReceiverGroupCommand createCommand)
    {
        var entity = createCommand.ReceiverGroup.Adapt<ReceiverGroup>();
        await _repository.AddAsync(entity);
        await _domainService.SetUsersAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateReceiverGroupCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.ReceiverGroupId);
        if (entity == null)
            throw new UserFriendlyException("receiverGroup not found");
        updateCommand.ReceiverGroup.Adapt(entity);
        await _repository.UpdateAsync(entity);
        await _domainService.SetUsersAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteReceiverGroupCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ReceiverGroupId);
        if (entity == null)
            throw new UserFriendlyException("receiverGroup not found");
        await _repository.RemoveAsync(entity);
    }
}