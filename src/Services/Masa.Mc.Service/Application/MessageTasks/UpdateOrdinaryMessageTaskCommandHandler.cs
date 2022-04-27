namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class UpdateOrdinaryMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTaskRepository _repository;
    private readonly IEventBus _eventBus;

    public UpdateOrdinaryMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTaskRepository repository, IEventBus eventBus)
    {
        _domainService = domainService;
        _repository = repository;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task UpdateMessageInfoAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var dto = updateCommand.MessageTask;
        var updateMessageInfoCommand = new UpdateMessageInfoCommand(dto.EntityId, dto.MessageInfo);
        await _eventBus.PublishAsync(updateMessageInfoCommand);
    }

    [EventHandler(2)]
    public async Task UpdateOrdinaryMessageTaskAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (!entity.IsDraft)
            throw new UserFriendlyException("non draft cannot be modified");
        var dto = updateCommand.MessageTask;
        dto.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }
}
