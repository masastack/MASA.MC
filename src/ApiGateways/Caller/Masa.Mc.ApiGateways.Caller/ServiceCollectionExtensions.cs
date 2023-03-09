// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcApiGateways(this IServiceCollection services, MasaOpenIdConnectOptions masaOpenIdConnectOptions, Action<McApiOptions>? configure = null)
    {
        var options = new McApiOptions("http://localhost:19501/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddSingleton(masaOpenIdConnectOptions);
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddAutoRegistrationCaller(Assembly.Load("Masa.Mc.ApiGateways.Caller"));
        return services;
    }
}