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

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, new[] { message.ClientId });

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage message, CancellationToken ct = default)
    {
        if (message.ClientIds.Length > 1000)
            return new AppNotificationResponseBase(false, "Up to 1000 device tokens can be sent at a time");

        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, message.ClientIds);

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage message, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _oauthService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HuaweiConstants.PushMessageUrlFormat, options.ProjectId);

        var payload = BuildMessagePayload(message, null, "all");

        var request = CreatePushRequest(url, payload, accessToken);

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
    }

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        // Huawei Push V1/V2 does not support message withdrawal  
        return Task.FromResult(new AppNotificationResponseBase(false, "Withdrawal operation not supported"));
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
                        ClickAction = new
                        {
                            type = string.IsNullOrEmpty(message.Url) ? (int)ClickActionType.OpenApp : (int)ClickActionType.AppDefinedIntent,
                            intent = message.Url
                        }
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

    private async Task<AppNotificationResponseBase> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<JsonElement>(options: null, cancellationToken: ct);
            var code = result.GetProperty("code").GetString();
            var msg = result.TryGetProperty("msg", out var msgProp) ? msgProp.GetString() : null;
            var requestId = result.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : null;

            if (code == "80000000")
            {
                return new AppNotificationResponseBase(true, "Success", requestId);
            }
            else if (code == "80100000")
            {
                // Some tokens are invalid  
                var illegalTokens = result.GetProperty("msg").GetProperty("illegal_tokens").EnumerateArray().Select(t => t.GetString()).ToList();
                return new AppNotificationResponseBase(false, $"Some tokens are invalid: {string.Join(",", illegalTokens)}", requestId);
            }
            else
            {
                return new AppNotificationResponseBase(false, $"Push failed: {msg}", requestId);
            }
        }

        var error = await response.Content.ReadAsStringAsync(ct);
        return new AppNotificationResponseBase(false, $"Push failed: HTTP {response.StatusCode} - {error}");
    }
}
