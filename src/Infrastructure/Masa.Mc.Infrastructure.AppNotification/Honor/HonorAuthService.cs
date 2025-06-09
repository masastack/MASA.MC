// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Honor;

public class HonorAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheContext _cacheContext;

    public HonorAuthService(HttpClient httpClient, ICacheContext cacheContext)
    {
        _httpClient = httpClient;
        _cacheContext = cacheContext;
    }

    public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, CancellationToken ct = default)
    {
        var cacheKey = $"honor:access_token:{clientId}";
        return await _cacheContext.GetOrSetAsync(cacheKey, async () =>
        {
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            };
            var content = new FormUrlEncodedContent(form);
            var response = await _httpClient.PostAsync(HonorConstants.OAuthTokenUrl, content, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("access_token").GetString()!;
            var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            var cacheEntryOptions = new CacheEntryOptions(TimeSpan.FromSeconds(Math.Max(expiresIn - 60, 60)));
            return (token, cacheEntryOptions);
        });
    }
}
