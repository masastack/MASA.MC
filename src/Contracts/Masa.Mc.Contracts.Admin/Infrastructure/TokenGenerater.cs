// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Infrastructure;

public class TokenGenerater : ITokenGenerater
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly IMasaStackConfig _masaStackConfig;
    private const string SCHEME = "Bearer ";

    public TokenGenerater(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IMasaStackConfig masaStackConfig)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _masaStackConfig = masaStackConfig;
    }

    public TokenProvider Generater()
    {
        StringValues authenticationHeaderValue;

        if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out authenticationHeaderValue) == true)
        {
            var accessToken = authenticationHeaderValue.ToString();

            if (!string.IsNullOrEmpty(accessToken) && accessToken.StartsWith(SCHEME, StringComparison.OrdinalIgnoreCase))
            {
                accessToken = accessToken.Substring(SCHEME.Length).Trim();
            }

            return new TokenProvider { AccessToken = accessToken };
        }

        if (_httpContextAccessor.HttpContext == null)
        {
            var request = new ClientCredentialsTokenRequest
            {
                Address = _masaStackConfig.GetSsoDomain() + "/connect/token",
                GrantType = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.CLIENT_CREDENTIALS,
                ClientId = _masaStackConfig.GetWebId(MasaStackProject.MC),
                Scope = "MasaStack"
            };
            var tokenResponse = _httpClient.RequestClientCredentialsTokenAsync(request).Result;
            return new TokenProvider { AccessToken = tokenResponse.AccessToken };
        }

        return new TokenProvider();
    }
}
