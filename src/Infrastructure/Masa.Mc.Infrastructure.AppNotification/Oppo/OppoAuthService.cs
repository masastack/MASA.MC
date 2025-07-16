// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public class OppoAuthService
{
    private readonly HttpClient _httpClient;

    public OppoAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync(string appKey, string masterSecret, CancellationToken ct)
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
        return doc.RootElement.GetProperty("data").GetProperty("auth_token").GetString()!;
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
