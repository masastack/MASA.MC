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
        var userIds = createCommand.ReceiverGroup.UserIds.ToArray();
        var items = createCommand.ReceiverGroup.Items.Adapt<List<ReceiverGroupItem>>();
        await _domainService.CreateAsync(entity, userIds, items);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateReceiverGroupCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.ReceiverGroupId);
        if (entity == null)
            throw new UserFriendlyException("receiverGroup not found");
        updateCommand.ReceiverGroup.Adapt(entity);
        var userIds = updateCommand.ReceiverGroup.UserIds.ToArray();
        var items = updateCommand.ReceiverGroup.Items.Adapt<List<ReceiverGroupItem>>();
        await _domainService.UpdateAsync(entity, userIds, items);
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