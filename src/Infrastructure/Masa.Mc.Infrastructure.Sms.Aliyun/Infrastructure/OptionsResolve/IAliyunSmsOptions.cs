namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public interface IAliyunSmsOptions
{
    public string AccessKeyId { get; set; }

    public string AccessKeySecret { get; set; }

    public string EndPoint { get; set; }
}