namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class SmsTemplateDto
{
    public Guid ChannelId { get; set; }
    public string TemplateCode { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public SmsTemplateType TemplateType { get; set; }
    public MessageTemplateAuditStatus AuditStatus { get; set; }
    public string TemplateContent { get; set; } = string.Empty;
    public string AuditReason { get; set; } = string.Empty;
}
