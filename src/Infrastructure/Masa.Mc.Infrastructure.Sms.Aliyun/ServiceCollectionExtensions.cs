namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAliyunSms(this IServiceCollection services, Action<AliyunSmsOptions> configureOptions)
    {
        return services.AddAliyunSms(string.Empty, configureOptions);
    }

    public static IServiceCollection AddAliyunSms(this IServiceCollection services, string name, Action<AliyunSmsOptions> configureOptions)
    {
        services.Configure(name,configureOptions);
        services.TryAddSingleton<ISmsSender, AliyunSmsSender>();
        return services;
    }

    public static IServiceCollection AddAliyunSms(this IServiceCollection services, AliyunSmsOptions options)
    {
        return services.AddAliyunSms(o => o.SetOptions(options));
    }

    public static IServiceCollection AddAliyunSms(this IServiceCollection services, string name, AliyunSmsOptions options)
    {
        return services.AddAliyunSms(name,o => o.SetOptions(options));
    }

    public static IServiceCollection AddAliyunSms(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AliyunSmsOptions>(string.Empty, configuration);
        services.TryAddSingleton<ISmsSender, AliyunSmsSender>();
        return services;
    }
}
