namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageInfoRepository _messageInfoRepository;

    public MessageTaskCommandHandler(IMessageTaskRepository repository, IIntegrationEventBus integrationEventBus, MessageTaskDomainService domainService, IMessageInfoRepository messageInfoRepositor)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
        _messageInfoRepository = messageInfoRepositor;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageTaskCommand createCommand)
    {
        var dto = createCommand.MessageTask;
        var entity = dto.Adapt<MessageTask>();
        if (dto.EntityType == MessageEntityType.Ordinary)
        {
            var messageInfo = dto.MessageInfo.Adapt<MessageInfo>();
            await _messageInfoRepository.AddAsync(messageInfo);
            entity.SetEntity(MessageEntityType.Ordinary, messageInfo.Id);
        }
        await _domainService.CreateAsync(entity);
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

    [EventHandler]
    public async Task ExecuteAsync(ExecuteMessageTaskCommand command)
    {
        var input = command.input;
        await _domainService.ExecuteAsync(input.MessageTaskId, input.ReceiverType, input.Receivers, input.SendingRules);
    }
}
