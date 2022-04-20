namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public class AliyunSmsOptionsResolveContext
{
    public IAliyunSmsOptions Options { get; set; }

    public IServiceProvider ServiceProvider { get; }

    public AliyunSmsOptionsResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
