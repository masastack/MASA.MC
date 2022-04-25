namespace Masa.Mc.Infrastructure.Sms.Services;

public interface ISmsTemplateService
{
    Task<SmsResponseBase> GetSmsTemplateAsync(string templateCode);

    Task<SmsResponseBase> GetSmsTemplateListAsync(int page = 1, int pageSize = 50);
}
