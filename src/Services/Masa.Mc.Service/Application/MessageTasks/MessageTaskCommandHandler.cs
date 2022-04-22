namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly MessageTaskDomainService _domainService;

    public MessageTaskCommandHandler(IMessageTaskRepository repository, IIntegrationEventBus integrationEventBus, MessageTaskDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageTaskCommand createCommand)
    {
        var entity = createCommand.MessageTask.Adapt<MessageTask>();
        await _repository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        updateCommand.MessageTask.Adapt(entity);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageTaskCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        await _repository.RemoveAsync(entity);
    }
}
