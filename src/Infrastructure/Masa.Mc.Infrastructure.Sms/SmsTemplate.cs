using Masa.Mc.Infrastructure.Sms.Response;

namespace Masa.Mc.Infrastructure.Sms
{
    public class SmsTemplate : ResponseBase
    {
        public string TemplateName { get; set; } = string.Empty;
        public string TemplateContent { get; set; } = string.Empty;

        public string TemplateCode { get; set; } = string.Empty;

        /// <summary>
        /// 0审核中1审核通过2审核失败
        /// </summary>
        public int? AuditStatus { get; set; }

        public int? TemplateType { get; set; }

        public string Reason { get; set; } = string.Empty;

        public SmsTemplate(bool success, string message, string json) : base(success, message, json)
        {

        }
    }
}
