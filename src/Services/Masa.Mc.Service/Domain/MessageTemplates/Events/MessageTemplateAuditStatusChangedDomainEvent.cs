namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record MessageTemplateAuditStatusChangedToApprovedDomainEvent(Guid TemplateId, string Remarks) : DomainEvent;

public record MessageTemplateAuditStatusChangedToRefuseDomainEvent(Guid TemplateId, string Remarks) : DomainEvent;

