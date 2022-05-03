﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiGateways(this IServiceCollection services, Action<McApiOptions>? configure = null)
    {
        var options = new McApiOptions("http://localhost:19501/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddCaller(Assembly.Load("Masa.Mc.Caller"));
        return services;
    }
}