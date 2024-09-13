// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Authentication;

public class TokenGenerater : ITokenGenerater
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;
    private readonly IMasaStackConfig _masaStackConfig;
    private const string SCHEME = "Bearer ";
    private readonly ICacheContext _cacheContext;

    public TokenGenerater(IHttpContextAccessor httpContextAccessor, HttpClient httpClient, IMasaStackConfig masaStackConfig, ICacheContext cacheContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
        _masaStackConfig = masaStackConfig;
        _cacheContext = cacheContext;
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
            var accessToken = GetClientCredentialsTokenAsync().Result;
            return new TokenProvider { AccessToken = accessToken };
        }

        return new TokenProvider();
    }

    private async Task<string> GetClientCredentialsTokenAsync()
    {
        var accessToken = await _cacheContext.GetOrSetAsync(CacheKeys.ClientCredentialsTokenKey(_masaStackConfig.GetWebId(MasaStackProject.MC)),
            async () =>
            {
                var request = new ClientCredentialsTokenRequest
                {
                    Address = _masaStackConfig.GetSsoDomain() + "/connect/token",
                    GrantType = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.CLIENT_CREDENTIALS,
                    ClientId = _masaStackConfig.GetWebId(MasaStackProject.MC),
                    Scope = BusinessConsts.COMMON_SCOPE
                };
                var tokenResponse = await _httpClient.RequestClientCredentialsTokenAsync(request);

                var cacheEntryOptions = new CacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60)
                };
                return (tokenResponse.AccessToken, cacheEntryOptions);
            }
            );

        return accessToken;
    }
}
