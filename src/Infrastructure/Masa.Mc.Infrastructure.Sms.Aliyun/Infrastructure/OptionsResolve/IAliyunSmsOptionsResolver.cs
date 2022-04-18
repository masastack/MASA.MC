namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public interface IAliyunSmsOptionsResolver
{
    Task<IAliyunSmsOptions> ResolveAsync();
}
