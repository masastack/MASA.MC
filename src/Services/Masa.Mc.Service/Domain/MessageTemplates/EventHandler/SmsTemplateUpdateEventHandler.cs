namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class SmsTemplateUpdateEventHandler
{
    [EventHandler]
    public void HandleEvent(SmsChannelChangedDomainEvent @event)
    {
        //After the SMS channel is changed, query the Alibaba cloud SMS interface and synchronize the SMS template
    }
}
