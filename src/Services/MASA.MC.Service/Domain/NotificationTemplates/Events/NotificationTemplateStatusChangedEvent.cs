namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Events
{
    public record NotificationTemplateStatusChangedToApprovedEvent(Guid TemplateId, string Remarks) : Event;

    public record NotificationTemplateStatusChangedToRefuseEvent(Guid TemplateId, string Remarks) : Event;
    
}
