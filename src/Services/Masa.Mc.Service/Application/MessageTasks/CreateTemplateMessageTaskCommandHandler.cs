namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class CreateTemplateMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public CreateTemplateMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTemplateRepository messageTemplateRepository)
    {
        _domainService = domainService;
        _messageTemplateRepository = messageTemplateRepository;
    }

    [EventHandler(1)]
    public async Task CheckMessageTemplateAsync(CreateTemplateMessageTaskCommand createCommand)
    {
        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == createCommand.MessageTask.EntityId);
        if (messageTemplate == null)
            throw new UserFriendlyException("messageTemplate not found");
        createCommand.MessageTask.DisplayName = string.IsNullOrEmpty(messageTemplate.Title) ? messageTemplate.DisplayName : messageTemplate.Title;
    }

    [EventHandler(2)]
    public async Task CreateTemplateMessageTaskAsync(CreateTemplateMessageTaskCommand createCommand)
    {
        var entity = createCommand.MessageTask.Adapt<MessageTask>();
        await _domainService.CreateAsync(entity);
    }
}
