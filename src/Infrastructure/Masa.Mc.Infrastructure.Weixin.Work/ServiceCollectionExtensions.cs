// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeixinWork(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<WeixinWorkResolveOptions>(options =>
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
        services.TryAddSingleton<IWeixinWorkAsyncLocalAccessor, WeixinWorkAsyncLocalAccessor>();
        services.TryAddTransient<IWeixinWorkAsyncLocal, WeixinWorkAsyncLocal>();
        services.TryAddTransient<IWeixinWorkOptionsResolver, WeixinWorkOptionsResolver>();
        services.AddMemoryCache();
        services.AddSenparcWeixinServices(configuration);
        services.TryAddSingleton<IWeixinWorkSender, WeixinWorkSender>();
        return services;
    }
}
