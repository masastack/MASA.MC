// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeixinMiniProgram(this IServiceCollection services)
    {
        services.Configure<ResolveOptions<IWeixinMiniProgramOptions>>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor<IWeixinMiniProgramOptions>.CONTRIBUTOR_NAME))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor<IWeixinMiniProgramOptions>());
            }
        });
        services.TryAddSingleton<IAsyncLocalAccessor<IWeixinMiniProgramOptions>, AsyncLocalAccessor<IWeixinMiniProgramOptions>>();
        services.TryAddTransient<IProviderAsyncLocal<IWeixinMiniProgramOptions>, ProviderAsyncLocal<IWeixinMiniProgramOptions>>();
        services.TryAddTransient<IOptionsResolver<IWeixinMiniProgramOptions>, WeixinMiniProgramOptionsResolver>();
        services.TryAddSingleton<IWeixinMiniProgramSender, WeixinMiniProgramSender>();
        services.TryAddSingleton<IWeixinMiniProgramTemplateProvider, WeixinMiniProgramTemplateProvider>();
        return services;
    }
}
