// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Firewall;

public static class FirewallServiceCollectionExtensions
{
    public static IServiceCollection AddFirewall(this IServiceCollection services, Func<IServiceProvider, FirewallOptions> func)
    {
        ArgumentNullException.ThrowIfNull(func, nameof(func));

        services.TryAddSingleton<IFirewallValidator>(serviceProvider
            => new DefaultFirewallValidator(func.Invoke(serviceProvider)));

        return services;
    }
}
