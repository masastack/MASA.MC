// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Infrastructure.OptionsResolve.Contributors;

public class ConfigurationOptionsResolveContributor : IAppNotificationOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "Configuration";
    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(AppNotificationOptionsResolveContext context)
    {
        context.Options = context.ServiceProvider.GetRequiredService<IOptions<AppOptions>>().Value;

        return Task.CompletedTask;
    }
}