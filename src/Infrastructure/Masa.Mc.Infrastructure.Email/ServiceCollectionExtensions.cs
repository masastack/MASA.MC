// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEmail(this IServiceCollection services)
    {
        services.Configure<EmailResolveOptions>(options =>
        {
            if (!options.Contributors.Exists(x => x.Name == ConfigurationOptionsResolveContributor.ContributorName))
            {
                options.Contributors.Add(new ConfigurationOptionsResolveContributor());
            }

            if (!options.Contributors.Exists(x => x.Name == AsyncLocalOptionsResolveContributor.ContributorName))
            {
                options.Contributors.Insert(0, new AsyncLocalOptionsResolveContributor());
            }
        });
        services.TryAddSingleton<IEmailAsyncLocalAccessor, EmailAsyncLocalAccessor>();
        services.TryAddTransient<IEmailAsyncLocal, EmailAsyncLocal>();
        services.TryAddTransient<IEmailOptionsResolver, EmailOptionsResolver>();
        services.TryAddSingleton<IEmailSender, SmtpEmailSender>();
        return services;
    }
}
