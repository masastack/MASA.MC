namespace Masa.Mc.Infrastructure.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, RedisConfigurationOptions redisOptions)
    {
        services.AddMultilevelCache(options => options.UseStackExchangeRedisCache(redisOptions));
        services.AddScoped<ICacheContext, CacheContext>();
        return services;
    }
}