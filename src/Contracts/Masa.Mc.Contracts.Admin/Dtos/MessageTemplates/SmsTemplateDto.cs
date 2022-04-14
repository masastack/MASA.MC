namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates
{
    public class SmsTemplateDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageTemplateAuditStatus AuditStatus { get; set; }
        public string AuditReason { get; set; } = string.Empty;
        public List<MessageTemplateItemDto> Items { get; set; }
        public int TemplateType { get; set; }
    }
}
