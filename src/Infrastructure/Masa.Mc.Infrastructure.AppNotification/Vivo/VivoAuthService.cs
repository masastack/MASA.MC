// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Vivo;

public class VivoAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ICacheContext _cacheContext;

    public VivoAuthService(HttpClient httpClient, ICacheContext cacheContext)
    {
        _httpClient = httpClient;
        _cacheContext = cacheContext;
    }

    public async Task<string> GetAccessTokenAsync(IVivoPushOptions options, CancellationToken ct = default)
    {
        var cacheKey = $"vivo:access_token:{options.AppId}";
        return await _cacheContext.GetOrSetAsync(cacheKey, async () =>
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var sign = GenerateSign(options.AppId, options.AppKey, timestamp, options.AppSecret);
            var payload = new
            {
                appId = options.AppId,
                appKey = options.AppKey,
                timestamp,
                sign
            };
            var request = new HttpRequestMessage(HttpMethod.Post, VivoConstants.AuthUrl)
            {
                Content = JsonContent.Create(payload)
            };
            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("authToken").GetString()!;
            var cacheEntryOptions = new CacheEntryOptions(TimeSpan.FromHours(23));
            return (token, cacheEntryOptions);
        });
    }

    private string GenerateSign(string appId, string appKey, long timestamp, string appSecret)
    {
        var raw = $"{appId}{appKey}{timestamp}{appSecret}";
        using var md5 = System.Security.Cryptography.MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(raw);
        var hash = md5.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
