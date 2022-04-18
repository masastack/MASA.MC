namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public class AliyunSmsResolveOptions
{
    public List<IAliyunSmsOptionsResolveContributor> Contributors { get; }

    public AliyunSmsResolveOptions()
    {
        Contributors = new List<IAliyunSmsOptionsResolveContributor>();
    }
}