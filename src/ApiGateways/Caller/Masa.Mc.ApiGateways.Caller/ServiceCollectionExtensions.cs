// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcApiGateways(this IServiceCollection services, Action<McApiOptions> configure)
    {
        services.AddSingleton<IResponseMessage, McResponseMessage>();
        var options = new McApiOptions();
        configure.Invoke(options);
        services.AddSingleton(options);
        services.AddStackCaller(Assembly.Load("Masa.Mc.ApiGateways.Caller"), (serviceProvider) => { return new TokenProvider(); }, jwtTokenValidatorOptions =>
        {
            jwtTokenValidatorOptions.AuthorityEndpoint = options.AuthorityEndpoint;
        }, clientRefreshTokenOptions =>
        {
            clientRefreshTokenOptions.ClientId = options.ClientId;
            clientRefreshTokenOptions.ClientSecret = options.ClientSecret;
        });
        return services;
    }
}