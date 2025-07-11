// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsTemplateService : ISmsTemplateService
{
    private readonly IOptionsResolver<IAliyunSmsOptions> _optionsResolver;

    public AliyunSmsTemplateService(IOptionsResolver<IAliyunSmsOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<SmsResponseBase> GetSmsTemplateAsync(string templateCode)
    {
        var client = await CreateClientAsync();
        var querySmsTemplateRequest = new QuerySmsTemplateRequest()
        {
            TemplateCode = templateCode
        };

        var response = await client.QuerySmsTemplateAsync(querySmsTemplateRequest);
        var body = response.Body;
        return new SmsTemplateResponse(body.Code == "OK", body.Message, response);
    }

    public async Task<SmsResponseBase> GetSmsTemplateListAsync(int page = 1, int pageSize = 50)
    {
        var client = await CreateClientAsync();
        var querySmsTemplateListRequest = new QuerySmsTemplateListRequest() { PageIndex = page, PageSize = pageSize };
        var response = await client.QuerySmsTemplateListAsync(querySmsTemplateListRequest);
        var body = response.Body;
        return new SmsTemplateListResponse(body.Code == "OK", body.Message, response);
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
