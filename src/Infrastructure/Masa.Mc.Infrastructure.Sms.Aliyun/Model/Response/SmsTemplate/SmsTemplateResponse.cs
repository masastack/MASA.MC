namespace Masa.Mc.Infrastructure.Sms.Aliyun.Model.Response.SmsTemplate;

public class SmsTemplateResponse : SmsResponseBase
{
    public QuerySmsTemplateResponse Data { get; set; }

    public SmsTemplateResponse(bool success, string message, QuerySmsTemplateResponse data) : base(success, message)
    {
        Data = data;
    }
}