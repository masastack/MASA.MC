// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly HuaweiOAuthService _oauthService;
    private readonly IOptionsResolver<IHuaweiPushOptions> _optionsResolver;

    public bool SupportsBroadcast => true;

    public HuaweiPushSender(HttpClient httpClient, HuaweiOAuthService oauthService, IOptionsResolver<IHuaweiPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _oauthService = oauthService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, new[] { message.ClientId });

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponse> BatchSendAsync(BatchAppMessage message, CancellationToken ct = default)
    {
        if (message.ClientIds.Length > 1000)
            return new AppNotificationResponse(false, "Up to 1000 device tokens can be sent at a time");

        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, message.ClientIds);

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponse> BroadcastSendAsync(AppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, null, AppNotificationConstants.BroadcastTag);

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        // Huawei Push V1/V2 does not support message withdrawal  
        return Task.FromResult(new AppNotificationResponse(false, "Withdrawal operation not supported"));
    }

    public async Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.TopicSubscribeUrlFormat, options.ProjectId);

        var payload = new
        {
            topic = name,
            token = new[] { clientId }
        };

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var result = await HandleResponse(response, ct);
        result.Message = result.Success
            ? "Subscribe topic succeeded"
            : $"Subscribe topic failed: {result.Message}";
        return result;
    }

    public async Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.TopicUnsubscribeUrlFormat, options.ProjectId);

        var payload = new
        {
            topic = name,
            token = new[] { clientId }
        };

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        var result = await HandleResponse(response, ct);
        result.Message = result.Success
            ? "Unsubscribe topic succeeded"
            : $"Unsubscribe topic failed: {result.Message}";
        return result;
    }


    private object BuildClickAction(string url)
    {
        if (string.IsNullOrEmpty(url))
            return new { type = 3 };
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return new { type = 2, url };
        return new { type = 1, intent = url };
    }

    private HmsPushRequest BuildMessagePayload(AppMessage message, string[]? tokens = null, string? topic = null)
    {
        return new HmsPushRequest
        {
            Message = new HmsMessage
            {
                Token = tokens ?? Array.Empty<string>(),
                Topic = topic ?? string.Empty,
                Notification = new HmsNotification { Title = message.Title, Body = message.Text },
                Data = JsonConvert.SerializeObject(message.TransmissionContent),
                Android = new HmsAndroidConfig
                {
                    Notification = new HmsAndroidNotification
                    {
                        ClickAction = BuildClickAction(message.Url)
                    }
                }
            }
        };
    }

    private HttpRequestMessage CreatePushRequest(string url, object payload, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return request;
    }

    private async Task<AppNotificationResponse> HandleResponse(HttpResponseMessage response, CancellationToken ct = default)
    {
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            return new AppNotificationResponse(false, $"HTTP {response.StatusCode}: {error}");
        }

        JsonElement root;
        try
        {
            var raw = await response.Content.ReadFromJsonAsync<JsonElement>();
            root = raw.ValueKind == JsonValueKind.String
                ? JsonDocument.Parse(raw.GetString()).RootElement
                : raw;
        }
        catch
        {
            return new AppNotificationResponse(false, "Invalid JSON");
        }

        string? code = null, msg = null, requestId = null;
        if (root.TryGetProperty("code", out var c) && c.ValueKind == JsonValueKind.String)
            code = c.GetString();

        if (root.TryGetProperty("msg", out var m))
            msg = m.ValueKind == JsonValueKind.String ? m.GetString() : m.ToString();

        if (root.TryGetProperty("requestId", out var r) && r.ValueKind == JsonValueKind.String)
            requestId = r.GetString();

        if (code == "80000000")
            return new AppNotificationResponse(true, "Success", requestId);

        if (code == "80100000" && TryGetIllegalTokens(msg, out var tokens))
            return new AppNotificationResponse(true, "Invalid token", requestId, tokens);

        return new AppNotificationResponse(false, $"Push failed: {msg}", requestId);
    }

    private bool TryGetIllegalTokens(string? msg, out List<string> tokens)
    {
        tokens = new();
        if (string.IsNullOrWhiteSpace(msg) || !msg.TrimStart().StartsWith("{")) return false;

        try
        {
            using var doc = JsonDocument.Parse(msg);
            var arr = doc.RootElement.GetProperty("illegal_tokens").EnumerateArray();
            tokens = arr.Select(t => t.GetString() ?? "").Where(t => t.Length > 0).ToList();
            return tokens.Count > 0;
        }
        catch
        {
            return false;
        }
    }
}
