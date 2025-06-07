// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppNotification(this IServiceCollection services, RedisConfigurationOptions redisOptions)
    {
        services.AddCache(redisOptions);
        services.AddGetui();
        services.AddJPush();
        services.AddHuaweiPush();
        services.AddXiaomiPush();
        services.TryAddTransient<AppNotificationSenderFactory>();
        return services;
    }

    public static IServiceCollection AddGetui(this IServiceCollection services) =>
        services.AddPushProvider<IGetuiOptions, GetuiOptionsResolver, GetuiSender, GeTuiSenderProvider>();

    public static IServiceCollection AddJPush(this IServiceCollection services) =>
        services.AddPushProvider<IJPushOptions, JPushOptionsResolver, JPushSender, JPushSenderProvider>();

    public static IServiceCollection AddHuaweiPush(this IServiceCollection services)
    {
        services.AddPushProvider<IHuaweiPushOptions, HuaweiPushOptionsResolver, HuaweiPushSender, HuaweiSenderProvider>();
        services.AddScoped<HuaweiOAuthService>();
        return services;
    }

    public static IServiceCollection AddXiaomiPush(this IServiceCollection services) =>
        services.AddPushProvider<IXiaomiPushOptions, XiaomiPushOptionsResolver, XiaomiPushSender, XiaomiSenderProvider>();

    public static IServiceCollection AddPushProvider<TOptions, TOptionsResolver, TSender, TSenderProvider>(
    this IServiceCollection services)
    where TOptions : class, IOptions
    where TOptionsResolver : class, IOptionsResolver<TOptions>
    where TSender : class, IAppNotificationSender
    where TSenderProvider : class, IAppNotificationSenderProvider
    {
        services.Configure<ResolveOptions<TOptions>>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor<TOptions>.CONTRIBUTOR_NAME))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor<TOptions>());
            }
        });
        services.TryAddSingleton<IAsyncLocalAccessor<TOptions>, AsyncLocalAccessor<TOptions>>();
        services.TryAddTransient<IProviderAsyncLocal<TOptions>, ProviderAsyncLocal<TOptions>>();
        services.TryAddTransient<IOptionsResolver<TOptions>, TOptionsResolver>();
        services.TryAddScoped<TSender>();
        services.AddTransient<IAppNotificationSenderProvider, TSenderProvider>();
        return services;
    }
}