namespace Lonsid.Fusion.Infrastructure.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddMultilevelCache(options => options.UseStackExchangeRedisCache());
        services.AddSingleton<ICacheContext, CacheContext>();
        return services;
    }
}