namespace MASA.MC.Service.Admin.Domain.MessageTemplates.Services;

public class MessageTemplateDomainService : DomainService
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateDomainService(IDomainEventBus eventBus, IMessageTemplateRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public async Task<MessageTemplate> DeleteAsync(MessageTemplate template)
    {
        if (template.IsStatic)
        {
            throw new UserFriendlyException("The template cannot be deleted");
        }

        return await _repository.RemoveAsync(template);
    }
}
