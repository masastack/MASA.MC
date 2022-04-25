namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class SmsTemplateCommandHandler
{
    private readonly ISmsTemplateRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly SmsTemplateDomainService _domainService;

    public SmsTemplateCommandHandler(ISmsTemplateRepository repository, IIntegrationEventBus integrationEventBus, SmsTemplateDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task SynchroAsync(SynchroSmsTemplateCommand command)
    {
        await _domainService.SynchroAsync(command.ChannelId);
    }
}
