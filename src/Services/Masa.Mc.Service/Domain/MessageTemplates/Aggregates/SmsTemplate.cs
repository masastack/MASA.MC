namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;

public class SmsTemplate : AuditAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }
    public string TemplateCode { get; protected set; } = string.Empty;
    public string TemplateName { get; protected set; } = string.Empty;
    public SmsTemplateType TemplateType { get; protected set; }
    public MessageTemplateAuditStatus AuditStatus { get; set; }
    public string TemplateContent { get; protected set; } = string.Empty;
    public string AuditReason { get; protected set; } = string.Empty;

    public SmsTemplate(Guid channelId, string templateCode, string templateName, SmsTemplateType templateType, MessageTemplateAuditStatus auditStatus, string templateContent, string auditReason)
    {
        ChannelId = channelId;
        TemplateCode = templateCode;
        TemplateName = templateName;
        TemplateType = templateType;
        AuditStatus = auditStatus;
        TemplateContent = templateContent;
        AuditReason = auditReason;
    }

    public void Update(string templateName, SmsTemplateType templateType, MessageTemplateAuditStatus auditStatus, string templateContent, string auditReason)
    {
        TemplateName = templateName;
        TemplateType = templateType;
        AuditStatus = auditStatus;
        TemplateContent = templateContent;
        AuditReason = auditReason;
    }
}
