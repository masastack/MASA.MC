namespace Masa.Mc.Infrastructure.Sms.Aliyun
{
    public class AliyunSmsSender : ISmsSender
    {
        protected AliyunSmsOptions Options { get; }

        public AliyunSmsSender(IOptionsSnapshot<AliyunSmsOptions> options)
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

        public async Task<SmsTemplate> GetSmsTemplateAsync(string templateCode)
        {
            var client = CreateClient();
            QuerySmsTemplateRequest querySmsTemplateRequest = new QuerySmsTemplateRequest()
            {
                TemplateCode = templateCode
            };
            var response = await client.QuerySmsTemplateAsync(querySmsTemplateRequest);
            var body = response.Body;
            return new SmsTemplate(body.Code == "OK", body.Message, JsonSerializer.Serialize(body))
            {
                TemplateName = body.TemplateName,
                TemplateContent = body.TemplateContent,
                TemplateCode = body.TemplateCode,
                AuditStatus = body.TemplateStatus,
                TemplateType = body.TemplateType,
                Reason = body.Reason
            };
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
    }
}