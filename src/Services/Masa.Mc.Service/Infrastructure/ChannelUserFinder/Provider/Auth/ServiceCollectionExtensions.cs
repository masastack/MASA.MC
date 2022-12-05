// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.ChannelUserFinder.Provider.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthChannelUserFinder(this IServiceCollection services)
    {
        services.AddScoped<IChannelUserFinder, AuthChannelUserFinder>();
        return services;
    }
}