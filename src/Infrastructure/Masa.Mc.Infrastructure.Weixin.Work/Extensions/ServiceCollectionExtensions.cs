// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeixinWork(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddWeixinWork();
        services.AddWorkWebhook();

        services.AddMemoryCache();
        services.AddSenparcWeixinServices(configuration);
        
        return services;
    }

    public static IServiceCollection AddWeixinWork(this IServiceCollection services)
    {
        services.Configure<ResolveOptions<IWeixinWorkOptions>>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor<IWeixinWorkOptions>.CONTRIBUTOR_NAME))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor<IWeixinWorkOptions>());
            }
        });
        services.TryAddSingleton<IAsyncLocalAccessor<IWeixinWorkOptions>, AsyncLocalAccessor<IWeixinWorkOptions>>();
        services.TryAddTransient<IProviderAsyncLocal<IWeixinWorkOptions>, ProviderAsyncLocal<IWeixinWorkOptions>>();
        services.TryAddTransient<IOptionsResolver<IWeixinWorkOptions>, WeixinWorkOptionsResolver>();
        services.TryAddSingleton<IWeixinWorkSender, WeixinWorkSender>();
        return services;
    }

    public static IServiceCollection AddWorkWebhook(this IServiceCollection services)
    {
        services.Configure<ResolveOptions<IWeixinWorkWebhookOptions>>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor<IWeixinWorkWebhookOptions>.CONTRIBUTOR_NAME))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor<IWeixinWorkWebhookOptions>());
            }
        });
        services.TryAddSingleton<IAsyncLocalAccessor<IWeixinWorkWebhookOptions>, AsyncLocalAccessor<IWeixinWorkWebhookOptions>>();
        services.TryAddTransient<IProviderAsyncLocal<IWeixinWorkWebhookOptions>, ProviderAsyncLocal<IWeixinWorkWebhookOptions>>();
        services.TryAddTransient<IOptionsResolver<IWeixinWorkWebhookOptions>, WeixinWorkWebhookOptionsResolver>();
        services.TryAddSingleton<IWeixinWorkWebhookSender, WeixinWorkWebhookSender>();
        return services;
    }
}
