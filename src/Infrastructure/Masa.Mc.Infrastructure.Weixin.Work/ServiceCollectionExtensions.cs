// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWeixinWork(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddMemoryCache();
        services.AddSenparcWeixinServices(configuration);
        services.TryAddSingleton<IWeixinWorkMessageSender, WeixinWorkMessageSender>();
        return services;
    }
}
