namespace Masa.Mc.Infrastructure.Sms.Model.Response;

public class SmsResponseBase
{
    public bool Success { get; }

    public string Message { get; }

    protected SmsResponseBase(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}
