namespace Masa.Mc.Infrastructure.Sms.Aliyun.Response.SmsTemplate
{
    public class SmsTemplateResponse : ResponseBase
    {
        public QuerySmsTemplateResponse Data { get; set; }

        public SmsTemplateResponse(bool success, string message, QuerySmsTemplateResponse data) : base(success, message)
        {
            Data = data;
        }
    }
}
