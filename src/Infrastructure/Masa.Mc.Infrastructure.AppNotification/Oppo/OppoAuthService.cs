// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public class OppoAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheContext _cacheContext;

    public OppoAuthService(HttpClient httpClient, ICacheContext cacheContext)
    {
        _httpClient = httpClient;
        _cacheContext = cacheContext;
    }

    public async Task<string> GetAccessTokenAsync(string appKey, string masterSecret, CancellationToken ct)
    {
        var cacheKey = $"oppo:access_token:{appKey}";

        return await _cacheContext.GetOrSetAsync(cacheKey, async () =>
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var timestamp = now;
            var sign = GenerateSign(appKey, masterSecret, timestamp);

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "app_key", appKey },
                { "sign", sign },
                { "timestamp", timestamp.ToString() }
            });

            var response = await _httpClient.PostAsync(OppoConstants.AuthUrl, content, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("data").GetProperty("auth_token").GetString()!;
            // OPPO official documentation states the validity period is 24 hours, it is recommended to refresh 1 hour in advance
            var cacheEntryOptions = new CacheEntryOptions(TimeSpan.FromHours(23));
            return (token, cacheEntryOptions);
        });
    }

    private string GenerateSign(string appKey, string masterSecret, long timestamp)
    {
        var raw = appKey + timestamp + masterSecret;
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
