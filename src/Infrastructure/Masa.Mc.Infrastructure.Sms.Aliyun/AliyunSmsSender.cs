namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender
{
    protected AliyunSmsOptions Options { get; }

    public AliyunSmsSender(IOptions<AliyunSmsOptions> options)
    {
        Options = options.Value;
    }

    public async Task SendAsync(SmsMessage smsMessage)
    {
        var client = CreateClient();

        await client.SendSmsAsync(new AliyunSendSmsRequest
        {
            PhoneNumbers = smsMessage.PhoneNumber,
            SignName = smsMessage.Properties["SignName"] as string,
            TemplateCode = smsMessage.Properties["TemplateCode"] as string,
            TemplateParam = smsMessage.Text
        });
    }

    public async Task<ResponseBase> GetSmsTemplateAsync(string templateCode)
    {
        var client = CreateClient();
        QuerySmsTemplateRequest querySmsTemplateRequest = new QuerySmsTemplateRequest()
        {
            TemplateCode = templateCode
        };
        
        var response = await client.QuerySmsTemplateAsync(querySmsTemplateRequest);
        var body = response.Body;
        return new SmsTemplateResponse(body.Code == "OK", body.Message, response);
    }

    protected virtual AliyunClient CreateClient()
    {
        return new(new AliyunConfig
        {
            AccessKeyId = Options.AccessKeyId,
            AccessKeySecret = Options.AccessKeySecret,
            Endpoint = Options.EndPoint
        });
    }

    public void SetOptions(IDictionary<string, object> options)
    {
        Options.SetOptions(new AliyunSmsOptions
        {
            AccessKeySecret = options["AccessKeySecret"].ToString(),
            AccessKeyId = options["AccessKeyId"].ToString(),
            EndPoint = options["EndPoint"].ToString(),
        });
    }
}
