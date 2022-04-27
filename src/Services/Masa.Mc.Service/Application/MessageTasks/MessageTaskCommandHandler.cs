namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IEventBus _eventBus;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , MessageTaskDomainService domainService
        , IMessageInfoRepository messageInfoRepositor
        , IMessageTemplateRepository messageTemplateRepository
        , IEventBus eventBus)
    {
        _repository = repository;
        _domainService = domainService;
        _messageInfoRepository = messageInfoRepositor;
        _messageTemplateRepository = messageTemplateRepository;
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageTaskCommand createCommand)
    {
        var dto = createCommand.MessageTask;
        var entity = dto.Adapt<MessageTask>();
        await HandleEntity(dto, entity, true);
        await _domainService.CreateAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (!entity.IsDraft)
            throw new UserFriendlyException("non draft cannot be modified");
        var dto = updateCommand.MessageTask;
        dto.Adapt(entity);
        await HandleEntity(dto, entity, false);
        await _domainService.UpdateAsync(entity);
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
    public async Task SendAsync(SendMessageTaskCommand command)
    {
        var input = command.input;
        await _domainService.SendAsync(input.Id, input.ReceiverType, ExtensionPropertyHelper.ObjMapToExtraProperty(input.Receivers), ExtensionPropertyHelper.ObjMapToExtraProperty(input.SendingRules), input.SendTime, input.Sign, input.Variables);
    }

    private async Task HandleEntity(MessageTaskCreateUpdateDto dto, MessageTask entity, bool isAdd)
    {
        switch (dto.EntityType)
        {
            case MessageEntityType.Ordinary:
                if (isAdd)
                {
                    var messageInfo = dto.MessageInfo.Adapt<MessageInfo>();
                    await _messageInfoRepository.AddAsync(messageInfo);
                    entity.SetEntity(MessageEntityType.Ordinary, messageInfo.Id, messageInfo.Title);
                }
                else
                {
                    var updateCommand = new UpdateMessageInfoCommand(dto.EntityId, dto.MessageInfo);
                    await _eventBus.PublishAsync(updateCommand);
                    entity.SetEntity(MessageEntityType.Ordinary, dto.EntityId, dto.MessageInfo.Title);
                }
                break;
            case MessageEntityType.Template:
                var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == dto.EntityId);
                if (messageTemplate == null)
                    throw new UserFriendlyException("messageTemplate not found");
                entity.SetEntity(MessageEntityType.Template, messageTemplate.Id, string.IsNullOrEmpty(messageTemplate.Title) ? messageTemplate.DisplayName : messageTemplate.Title);
                break;
            default:
                break;
        }
    }
}
