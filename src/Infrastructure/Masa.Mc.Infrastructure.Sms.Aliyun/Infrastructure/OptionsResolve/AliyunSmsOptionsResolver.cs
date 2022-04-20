namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve;

public class AliyunSmsOptionsResolver : IAliyunSmsOptionsResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AliyunSmsResolveOptions _options;

    public AliyunSmsOptionsResolver(IServiceProvider serviceProvider,
        IOptions<AliyunSmsResolveOptions> aliyunSmsResolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = aliyunSmsResolveOptions.Value;
    }

    public async Task<IAliyunSmsOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new AliyunSmsOptionsResolveContext(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new AliyunSmsOptions();
    }
}
