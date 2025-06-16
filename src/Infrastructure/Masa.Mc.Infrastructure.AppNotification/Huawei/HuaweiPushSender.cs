// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly HuaweiOAuthService _oauthService;
    private readonly IOptionsResolver<IHuaweiPushOptions> _optionsResolver;

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

    private object BuildClickAction(string url)
    {
        if (string.IsNullOrEmpty(url))
            return new { type = 3 };
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return new { type = 2, url };
        return new { type = 1, action = url };
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

    private async Task<AppNotificationResponse> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(options: null, cancellationToken: ct);
            var code = result.GetProperty("code").GetString();
            var msg = result.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : null;
            var requestId = result.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : null;

            if (code == "80000000")
            {
                return new AppNotificationResponse(true, "Success", requestId);
            }
            else if (code == "80100000")
            {
                var illegalTokens = result.GetProperty("msg").GetProperty("illegal_tokens")
                    .EnumerateArray().Select(t => t.GetString() ?? string.Empty).ToList();
                return new AppNotificationResponse(true, $"Some tokens are invalid: {string.Join(",", illegalTokens)}", requestId, illegalTokens);
            }
            else
            {
                return new AppNotificationResponse(false, $"Push failed: {msg}", requestId);
            }
        }

        var error = await response.Content.ReadAsStringAsync(ct);
        return new AppNotificationResponse(false, $"Push failed: HTTP {response.StatusCode} - {error}");
    }
}
