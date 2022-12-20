// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Getui;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGetui(this IServiceCollection services)
    {
        services.Configure<GetuiResolveOptions>(options =>
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
        services.TryAddSingleton<IAppNotificationAsyncLocalAccessor, GetuiAsyncLocalAccessor>();
        services.TryAddTransient<IAppNotificationAsyncLocal, GetuiAsyncLocal>();
        services.TryAddTransient<IAppNotificationOptionsResolver, GetuiOptionsResolver>();
        services.TryAddSingleton<IAppNotificationSender, GetuiSender>();
        return services;
    }
}