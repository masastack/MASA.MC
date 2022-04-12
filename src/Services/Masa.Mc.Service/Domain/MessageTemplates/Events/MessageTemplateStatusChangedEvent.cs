namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record MessageTemplateStatusChangedToApprovedEvent(Guid TemplateId, string Remarks) : Event;

public record MessageTemplateStatusChangedToRefuseEvent(Guid TemplateId, string Remarks) : Event;

