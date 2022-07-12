// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.MailKit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMailKit(this IServiceCollection services)
    {
        services.Configure<EmailResolveOptions>(options =>
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
        services.TryAddSingleton<IEmailAsyncLocalAccessor, EmailAsyncLocalAccessor>();
        services.TryAddTransient<IEmailAsyncLocal, EmailAsyncLocal>();
        services.TryAddTransient<IEmailOptionsResolver, EmailOptionsResolver>();
        services.TryAddSingleton<IEmailSender, MailKitSmtpEmailSender>();
        return services;
    }
}
