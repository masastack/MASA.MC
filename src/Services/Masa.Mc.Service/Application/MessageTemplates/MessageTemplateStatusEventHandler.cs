namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateStatusEventHandler
{
    private readonly IEventBus _eventBus;

    public MessageTemplateStatusEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToApproved(MessageTemplateAuditStatusChangedToApprovedEvent integrationEvent)
    {
        await _eventBus.PublishAsync(new UpdateMessageTemplateCommand(integrationEvent.TemplateId,new MessageTemplateCreateUpdateDto
        {
            AuditStatus = MessageTemplateAuditStatus.Adopt
        }));
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToRefuse(MessageTemplateAuditStatusChangedToRefuseEvent integrationEvent)
    {
        await _eventBus.PublishAsync(new UpdateMessageTemplateCommand(integrationEvent.TemplateId, new MessageTemplateCreateUpdateDto
        {
            AuditStatus = MessageTemplateAuditStatus.Fail
        }));
    }
}
