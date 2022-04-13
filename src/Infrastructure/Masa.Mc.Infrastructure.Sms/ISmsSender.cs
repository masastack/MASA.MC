namespace Masa.Mc.Infrastructure.Sms;

public interface ISmsSender
{
    void SetOptions(IDictionary<string, object> options);

    Task SendAsync(SmsMessage smsMessage);

    Task<SmsTemplate> GetSmsTemplateAsync(string templateCode);
}
