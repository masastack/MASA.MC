using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates
{
    public class GetSmsTemplateDto
    {
        public string DisplayName { get; set; } = string.Empty;
        public string TemplateId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageTemplateAuditStatus AuditStatus { get; set; }
        public string AuditReason { get; set; } = string.Empty;
        public List<MessageTemplateItemDto> Items { get; set; }
    }
}
