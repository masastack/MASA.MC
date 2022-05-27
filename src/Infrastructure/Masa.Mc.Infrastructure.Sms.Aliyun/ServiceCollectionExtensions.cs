// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAliyunSms(this IServiceCollection services)
    {
        services.Configure<AliyunSmsResolveOptions>(options =>
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
        services.TryAddSingleton<IAliyunSmsAsyncLocalAccessor, AliyunSmsAsyncLocalAccessor>();
        services.TryAddTransient<IAliyunSmsAsyncLocal, AliyunSmsAsyncLocal>();
        services.TryAddTransient<IAliyunSmsOptionsResolver, AliyunSmsOptionsResolver>();
        services.TryAddSingleton<ISmsSender, AliyunSmsSender>();
        services.TryAddSingleton<ISmsTemplateService, SmsTemplateService>();
        return services;
    }
}