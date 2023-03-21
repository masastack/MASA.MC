// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppNotification(this IServiceCollection services)
    {
        services.Configure<AppResolveOptions>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == ConfigurationOptionsResolveContributor.CONTRIBUTOR_NAME))
            {
                options.Contributors.Add(new ConfigurationOptionsResolveContributor());
            }

            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor.CONTRIBUTOR_NAME))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor());
            }
        });
        services.TryAddSingleton<IAppNotificationAsyncLocalAccessor, AppAsyncLocalAccessor>();
        services.TryAddTransient<IAppNotificationAsyncLocal, AppAsyncLocal>();
        services.TryAddTransient<IAppNotificationOptionsResolver, AppOptionsResolver>();
        services.AddTransient<AppNotificationSenderFactory>();
        services.AddTransient<GetuiSender>(); 
        services.AddTransient<JPushSender>();
        return services;
    }
}