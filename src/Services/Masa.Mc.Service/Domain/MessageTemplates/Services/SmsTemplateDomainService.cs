namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public class SmsTemplateDomainService : DomainService
{
    public SmsTemplateDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    [EventHandler]
    public void SynchronousAsync(SmsChannelChangedDomainEvent @event)
    {
        //After the SMS channel is changed, query the Alibaba cloud SMS interface and synchronize the SMS template
    }
}
