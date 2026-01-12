// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmsSender(this IServiceCollection services)
    {
        services.AddAliyunSms();
        services.AddYunMasSms();
        services.TryAddTransient<SmsSenderFactory>();
        return services;
    }

    public static IServiceCollection AddAliyunSms(this IServiceCollection services)
    {
        services.AddSmsProvider<IAliyunSmsOptions, AliyunSmsOptionsResolver, AliyunSmsSender, AliyunSmsSenderProvider>();
        services.TryAddSingleton<ISmsTemplateService, AliyunSmsTemplateService>();
        return services;
    }
    public static IServiceCollection AddYunMasSms(this IServiceCollection services) =>
        services.AddSmsProvider<IYunMasOptions, YunMasOptionsResolver, YunMasSmsSender, YunMasSmsSenderProvider>();

    public static IServiceCollection AddSmsProvider<TOptions, TOptionsResolver, TSender, TSenderProvider>(
    this IServiceCollection services)
    where TOptions : class, IOptions
    where TOptionsResolver : class, IOptionsResolver<TOptions>
    where TSender : class, ISmsSender
        where TSenderProvider : class, ISmsSenderProvider
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
        services.AddTransient<ISmsSenderProvider, TSenderProvider>();
        return services;
    }
}