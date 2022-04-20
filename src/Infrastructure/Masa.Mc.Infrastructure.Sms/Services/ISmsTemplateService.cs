namespace Masa.Mc.Infrastructure.Sms.Services;

public interface ISmsTemplateService
{
    Task<SmsResponseBase> GetSmsTemplateAsync(string templateCode);
}
