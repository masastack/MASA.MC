// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcApiGateways(this IServiceCollection services, Action<McApiOptions>? configure = null)
    {
        var options = new McApiOptions("http://localhost:19501/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddAutoRegistrationCaller(Assembly.Load("Masa.Mc.ApiGateways.Caller"));
        return services;
    }

    public static IServiceCollection AddJwtTokenValidator(this IServiceCollection services,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptions, Action<ClientRefreshTokenOptions> clientRefreshTokenOptions)
    {
        var options = new JwtTokenValidatorOptions();
        jwtTokenValidatorOptions.Invoke(options);
        services.AddSingleton(options);
        var refreshTokenOptions = new ClientRefreshTokenOptions();
        clientRefreshTokenOptions.Invoke(refreshTokenOptions);
        services.AddSingleton(refreshTokenOptions);
        services.AddScoped<JwtTokenValidator>();
        return services;
    }
}