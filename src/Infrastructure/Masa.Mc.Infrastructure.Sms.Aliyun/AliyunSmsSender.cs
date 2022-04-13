using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using AliyunClient = AlibabaCloud.SDK.Dysmsapi20170525.Client;
using AliyunConfig = AlibabaCloud.OpenApiClient.Models.Config;
using AliyunSendSmsRequest = AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest;

namespace Masa.Mc.Infrastructure.Sms.Aliyun
{
    public class AliyunSmsSender : ISmsSender
    {
        protected AliyunSmsOptions Options { get; }

        public AliyunSmsSender(AliyunSmsOptions options)
        {
            Options = options;
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