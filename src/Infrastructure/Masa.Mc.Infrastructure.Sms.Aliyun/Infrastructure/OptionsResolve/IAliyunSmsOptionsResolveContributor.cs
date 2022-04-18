namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public interface IAliyunSmsOptionsResolveContributor
{
    string Name { get; }

    Task ResolveAsync(AliyunSmsOptionsResolveContext context);
}
