// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.MessageTaskJobService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageTaskAppJobService(this IServiceCollection services)
    {
        services.AddScoped<IMessageTaskJobService, MessageTaskAppJobService>();
        return services;
    }

    public static IServiceCollection AddMessageTaskHttpJobService(this IServiceCollection services)
    {
        services.AddScoped<IMessageTaskJobService, MessageTaskHttpJobService>();
        return services;
    }
}
