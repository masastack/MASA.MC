namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve.Contributors;

public class ConfigurationOptionsResolveContributor : IAliyunSmsOptionsResolveContributor
{
    public const string ContributorName = "Configuration";
    public string Name => ContributorName;

    public Task ResolveAsync(AliyunSmsOptionsResolveContext context)
    {
        context.Options = context.ServiceProvider.GetRequiredService<IOptions<AliyunSmsOptions>>().Value;

        return Task.CompletedTask;
    }
}