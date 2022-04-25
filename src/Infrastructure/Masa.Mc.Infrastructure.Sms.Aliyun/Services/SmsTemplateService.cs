namespace Masa.Mc.Infrastructure.Sms.Aliyun.Services;

public class SmsTemplateService : ISmsTemplateService
{
    private readonly IAliyunSmsOptionsResolver _aliyunSmsOptionsResolver;

    public SmsTemplateService(IAliyunSmsOptionsResolver aliyunSmsOptionsResolver)
    {
        _aliyunSmsOptionsResolver = aliyunSmsOptionsResolver;
    }

    public async Task<SmsResponseBase> GetSmsTemplateAsync(string templateCode)
    {
        var client = await CreateClientAsync();
        QuerySmsTemplateRequest querySmsTemplateRequest = new QuerySmsTemplateRequest()
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
        QuerySmsTemplateListRequest querySmsTemplateListRequest = new QuerySmsTemplateListRequest() { PageIndex = page, PageSize = pageSize };
        var response = await client.QuerySmsTemplateListAsync(querySmsTemplateListRequest);
        var body = response.Body;
        return new SmsTemplateListResponse(body.Code == "OK", body.Message, response);
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
