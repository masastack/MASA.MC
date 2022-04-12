namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record MessageTemplateAuditStatusChangedToApprovedEvent(Guid TemplateId, string Remarks) : Event;

public record MessageTemplateAuditStatusChangedToRefuseEvent(Guid TemplateId, string Remarks) : Event;

