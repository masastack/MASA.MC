namespace Masa.Mc.Infrastructure.Sms.Response;

public class ResponseBase
{
    public bool Success { get; }

    public string Message { get; } 

    protected ResponseBase(bool success, string message)
    {
        Success = success;
        Message = message;
    }
}
