// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiOAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HuaweiOAuthService> _logger;
    private readonly ICacheContext _cacheContext;

    public HuaweiOAuthService(
        HttpClient httpClient,
        ILogger<HuaweiOAuthService> logger,
        ICacheContext cacheContext)
    {
        _httpClient = httpClient;
        _logger = logger;
        _cacheContext = cacheContext;
    }

    public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, CancellationToken ct = default)
    {
        var cacheKey = $"huawei:access_token:{clientId}";

        // Use cache or retrieve again
        return await _cacheContext.GetOrSetAsync(cacheKey,
           async () =>
           {
               var tokenResponse = await RefreshTokenInternalAsync(clientId, clientSecret, ct);

               var cacheEntryOptions = new CacheEntryOptions(TimeSpan.FromSeconds(tokenResponse.ExpiresIn - 60));
               return (tokenResponse.AccessToken, cacheEntryOptions);
           }
           );
    }

    private async Task<AccessTokenResponse> RefreshTokenInternalAsync(string clientId, string clientSecret, CancellationToken ct)
    {
        var formData = new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        };

        var content = new FormUrlEncodedContent(formData);

        var response = await _httpClient.PostAsync(HuaweiConstants.OAuthTokenUrl, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(ct);
            _logger.LogWarning("Huawei OAuth returned status code {StatusCode}, body: {ErrorBody}",
                response.StatusCode, errorBody);
            throw new HttpRequestException($"Huawei OAuth failed with status code {response.StatusCode}: {errorBody}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync(ct);
        var tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(jsonResponse);

        if (tokenResponse == null)
        {
            throw new InvalidOperationException("Failed to deserialize Huawei access token response.");
        }

        return tokenResponse;
    }

    private class AccessTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null!;
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}