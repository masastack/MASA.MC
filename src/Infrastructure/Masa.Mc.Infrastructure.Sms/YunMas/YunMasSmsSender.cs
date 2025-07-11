namespace Masa.Mc.Infrastructure.Sms;

public class YunMasSmsSender : ISmsSender
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsResolver<IYunMasOptions> _optionsResolver;

    public YunMasSmsSender(IOptionsResolver<IYunMasOptions> optionsResolver, HttpClient httpClient)
    {
        _optionsResolver = optionsResolver;
        _httpClient = httpClient;
    }

    public bool SupportsTemplate => false;

    public async Task<SmsResponseBase> SendAsync(SmsMessage smsMessage)
    {
        var options = await _optionsResolver.ResolveAsync();
        var mac = YunMasUtil.BuildMac(options.EcName, options.ApId, options.SecretKey, smsMessage.PhoneNumber, smsMessage.Text, options.Sign, options.AddSerial);

        var req = new YunMasRequest
        {
            ecName = options.EcName,
            apId = options.ApId,
            mobiles = smsMessage.PhoneNumber,
            content = smsMessage.Text,
            sign = options.Sign,
            addSerial = options.AddSerial,
            mac = mac
        };

        var base64Body = YunMasUtil.ToBase64Json(req);
        var httpContent = new StringContent(base64Body, Encoding.UTF8, "text/plain");
        var url = string.Format(YunMasConstants.SmsSendUrlFormat, options.ApiUrl);

        var response = await _httpClient.PostAsync(url, httpContent);
        var respJson = await response.Content.ReadAsStringAsync();
        var respObj = JsonSerializer.Deserialize<YunMasSmsSendResponse>(respJson);
        if (respObj == null)
            return new SmsResponseBase(false, "Response parsing failed", string.Empty);

        return new SmsResponseBase(respObj.Success, respObj.Rspcod, respObj.MgsGroup);
    }

    public async Task<SmsResponseBase> SendBatchAsync(BatchSmsMessage smsMessage)
    {
        var options = await _optionsResolver.ResolveAsync();
        var mac = YunMasUtil.BuildMac(options.EcName, options.ApId, options.SecretKey, "", smsMessage.Text, options.Sign, options.AddSerial);

        var req = new
        {
            ecName = options.EcName,
            apId = options.ApId,
            mobiles = "",
            content = smsMessage.Text,
            sign = options.Sign,
            addSerial = options.AddSerial,
            mac
        };

        var base64Body = YunMasUtil.ToBase64Json(req);
        var httpContent = new StringContent(base64Body, Encoding.UTF8, "text/plain");
        var url = string.Format(YunMasConstants.SmsSendUrlFormat, options.ApiUrl);

        var response = await _httpClient.PostAsync(url, httpContent);
        var respJson = await response.Content.ReadAsStringAsync();
        var respObj = JsonSerializer.Deserialize<YunMasSmsSendResponse>(respJson);

        if (respObj == null)
            return new SmsResponseBase(false, "Response parsing failed", string.Empty);

        return new SmsResponseBase(respObj.Success, respObj.Rspcod, respObj.MgsGroup);
    }
}
