namespace Masa.Mc.Infrastructure.Sms;

public interface ISmsSender
{
    Task SendAsync(SmsMessage smsMessage);

    Task<SmsTemplate> GetSmsTemplateAsync(string templateCode);
}
