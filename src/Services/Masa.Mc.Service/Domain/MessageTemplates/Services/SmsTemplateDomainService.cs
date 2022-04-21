namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public class SmsTemplateDomainService : DomainService
{
    private readonly IMessageTemplateRepository _repository;

    public SmsTemplateDomainService(IDomainEventBus eventBus, IMessageTemplateRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public async Task SynchroAsync(Guid channelId)
    {
        await EventBus.PublishAsync(new SmsTemplateSynchroDomainEvent(channelId));
    }
}
