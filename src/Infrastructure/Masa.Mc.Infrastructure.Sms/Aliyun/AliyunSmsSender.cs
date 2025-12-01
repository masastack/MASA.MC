// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsSender : ISmsSender
{
    private readonly IOptionsResolver<IAliyunSmsOptions> _optionsResolver;
    private readonly ILogger<AliyunSmsSender> _logger;

    public AliyunSmsSender(IOptionsResolver<IAliyunSmsOptions> optionsResolver, ILogger<AliyunSmsSender> logger)
    {
        _optionsResolver = optionsResolver;
        _logger = logger;
    }

    public bool SupportsTemplate => true;

    public async Task<SmsResponseBase> SendAsync(SmsMessage smsMessage)
    {
        var client = await CreateClientAsync();

        var templateCode = smsMessage.Properties["TemplateCode"] as string;
        var signName = smsMessage.Properties["SignName"] as string;

        var response = await client.SendSmsAsync(new AliyunSendSmsRequest
        {
            PhoneNumbers = smsMessage.PhoneNumber,
            SignName = signName,
            TemplateCode = templateCode,
            TemplateParam = smsMessage.Text
        });
        var success = response.Body.Code == "OK";
        var rawJson = JsonSerializer.Serialize(response.Body);
        LogAliyunSendResult(false, success, response.Body.Message, rawJson);
        return new SmsSendResponse(success, response.Body.Message, response.Body.BizId, response);
    }

    public async Task<SmsResponseBase> SendBatchAsync(BatchSmsMessage smsMessage)
    {
        var client = await CreateClientAsync();

        var signNameValue = smsMessage.Properties["SignName"] as string;
        var signName = smsMessage.PhoneNumbers.Select(x => signNameValue).ToList();
        var templateCode = smsMessage.Properties["TemplateCode"] as string;

        var request = new SendBatchSmsRequest
        {
            PhoneNumberJson = JsonSerializer.Serialize(smsMessage.PhoneNumbers),
            SignNameJson = JsonSerializer.Serialize(signName),
            TemplateCode = templateCode,
            TemplateParamJson = smsMessage.Text
        };

        var response = await client.SendBatchSmsAsync(request);
        var batchSuccess = response.Body.Code == "OK";
        var rawJson = JsonSerializer.Serialize(response.Body);
        LogAliyunSendResult(true, batchSuccess, response.Body.Message, rawJson);
        return new SmsResponseBase(batchSuccess, response.Body.Message, response.Body.BizId);
    }

    private void LogAliyunSendResult(bool isBatch, bool success, string? message, string rawResponseJson)
    {
        if (success)
        {
            _logger.LogInformation("SMS send {Mode} success: {Message}. RawResponse={RawResponse}", isBatch ? "batch" : "single", message, rawResponseJson);
        }
        else
        {
            _logger.LogWarning("SMS send {Mode} failed: {Message}. RawResponse={RawResponse}", isBatch ? "batch" : "single", message, rawResponseJson);
        }
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
