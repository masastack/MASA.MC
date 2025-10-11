// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender
{
    private readonly IOptionsResolver<IAliyunSmsOptions> _optionsResolver;

    public AliyunSmsSender(IOptionsResolver<IAliyunSmsOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public bool SupportsTemplate => true;

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
        return new SmsSendResponse(response.Body.Code == "OK", response.Body.Message, response.Body.BizId, response);
    }

    public async Task<SmsResponseBase> SendBatchAsync(BatchSmsMessage smsMessage)
    {
        var client = await CreateClientAsync();

        var signName = smsMessage.PhoneNumbers.Select(x => smsMessage.Properties["SignName"] as string).ToList();

        var request = new SendBatchSmsRequest
        {
            PhoneNumberJson = JsonSerializer.Serialize(smsMessage.PhoneNumbers),
            SignNameJson = JsonSerializer.Serialize(signName),
            TemplateCode = smsMessage.Properties["TemplateCode"] as string,
            TemplateParamJson = smsMessage.Text
        };

        var response = await client.SendBatchSmsAsync(request);
        return new SmsResponseBase(response.Body.Code == "OK", response.Body.Message, response.Body.BizId);
    }

    protected async Task<AliyunClient> CreateClientAsync()
    {
        var options = await _optionsResolver.ResolveAsync();
        return new(new AliyunConfig
        {
            AccessKeyId = options.AccessKeyId,
            AccessKeySecret = options.AccessKeySecret,
            Endpoint = options.EndPoint
        });
    }
}
