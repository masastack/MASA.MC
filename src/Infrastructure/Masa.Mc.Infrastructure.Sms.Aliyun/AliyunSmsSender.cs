// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender
{
    private readonly IAliyunSmsOptionsResolver _aliyunSmsOptionsResolver;

    public AliyunSmsSender(IAliyunSmsOptionsResolver aliyunSmsOptionsResolver)
    {
        _aliyunSmsOptionsResolver = aliyunSmsOptionsResolver;
    }

    public async Task<SmsResponseBase> SendAsync(SmsMessage smsMessage)
    {
        var client = await CreateClientAsync();

        var response = await client.SendSmsAsync(new AliyunSendSmsRequest
        {
            PhoneNumbers = smsMessage.PhoneNumber,
            SignName = smsMessage.Properties["SignName"] as string,
            TemplateCode = smsMessage.Properties["TemplateCode"] as string,
            TemplateParam = smsMessage.Text
        });
        return new SmsSendResponse(response.Body.Code == "OK", response.Body.Message, response);
    }

    protected async Task<AliyunClient> CreateClientAsync()
    {
        var options = await _aliyunSmsOptionsResolver.ResolveAsync();
        return new(new AliyunConfig
        {
            AccessKeyId = options.AccessKeyId,
            AccessKeySecret = options.AccessKeySecret,
            Endpoint = options.EndPoint
        });
    }
}
