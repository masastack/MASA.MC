namespace MASA.MC.Service.Admin.Application.MessageTemplates;

public class MessageTemplateStatusEventHandler
{
    private readonly IEventBus _eventBus;

    public MessageTemplateStatusEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToApproved(MessageTemplateStatusChangedToApprovedEvent integrationEvent)
    {
        await _eventBus.PublishAsync(new UpdateMessageTemplateCommand(integrationEvent.TemplateId, new MessageTemplateCreateUpdateDto
        {
            Status = MessageTemplateStatus.Approved
        }));
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToRefuse(MessageTemplateStatusChangedToRefuseEvent integrationEvent)
    {
        await _eventBus.PublishAsync(new UpdateMessageTemplateCommand(integrationEvent.TemplateId, new MessageTemplateCreateUpdateDto
        {
            Status = MessageTemplateStatus.Refuse
        }));
    }
}
