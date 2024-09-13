namespace Masa.Mc.Infrastructure.Cache;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddMultilevelCache(options => options.UseStackExchangeRedisCache());
        services.AddScoped<ICacheContext, CacheContext>();
        return services;
    }
}