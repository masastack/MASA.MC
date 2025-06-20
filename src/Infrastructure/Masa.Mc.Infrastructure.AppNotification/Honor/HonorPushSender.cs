// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Masa.Mc.Infrastructure.AppNotification.Honor;

public class HonorPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly HonorAuthService _authService;
    private readonly IOptionsResolver<IHonorPushOptions> _optionsResolver;

    public bool SupportsBroadcast => false;

    public bool SupportsReceipt => true;

    public HonorPushSender(HttpClient httpClient, HonorAuthService authService, IOptionsResolver<IHonorPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _authService = authService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var response = await SendInternalAsync(appMessage, new[] { appMessage.ClientId }, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<IEnumerable<AppNotificationResponse>> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length > 1000)
            return appMessage.ClientIds.Select(x => new AppNotificationResponse(false, "Up to 1000 device tokens can be sent at a time", string.Empty, x));

        var response = await SendInternalAsync(appMessage, appMessage.ClientIds, ct);
        return await HandleBatchResponse(response, appMessage.ClientIds, ct);
    }

    public Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "Honor Push does not support broadcast send"));

    public Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "Honor Push does not support message withdrawal"));

    public Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support subscribe"));

    public Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support unsubscribe"));

    private async Task<HttpResponseMessage> SendInternalAsync(AppMessage appMessage, string[] clientIds, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();
        var accessToken = await _authService.GetAccessTokenAsync(options.ClientId, options.ClientSecret, ct);
        var url = string.Format(HonorConstants.PushMessageUrlFormat, options.AppId);

        var payload = BuildPayload(appMessage, clientIds);

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Add("timestamp", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString());

        return await _httpClient.SendAsync(request, ct);
    }

    private object BuildPayload(AppMessage message, string[] tokens)
    {
        if (tokens == null || tokens.Length == 0)
            throw new ArgumentException("tokens must not be null or empty for Honor push.");

        return new
        {
            token = tokens,
            notification = new
            {
                title = message.Title,
                body = message.Text
            },
            data = message.TransmissionContent?.Count > 0
                ? JsonSerializer.Serialize(message.TransmissionContent)
                : null,
            android = new
            {
                notification = new
                {
                    title = message.Title,
                    body = message.Text,
                    clickAction = BuildClickAction(message.Url)
                }
            }
        };
    }

    private object BuildClickAction(string url)
    {
        return string.IsNullOrEmpty(url)
            ? new { type = HonorClickActionType.OpenApp }
            : url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? new { type = HonorClickActionType.OpenUrl, url }
                : new { type = HonorClickActionType.AppDefinedIntent, intent = url };
    }

    private async Task<AppNotificationResponse> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            var code = doc.RootElement.GetProperty("code").GetInt32();
            var message = doc.RootElement.GetProperty("message").GetString() ?? string.Empty;
            var data = doc.RootElement.TryGetProperty("data", out var dataProp) ? dataProp : default;
            var requestId = data.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : string.Empty;
            var sendResult = data.TryGetProperty("sendResult", out var sendResultProp) && sendResultProp.GetBoolean();

            return code == 200 && sendResult
                ? new AppNotificationResponse(true, "Success", requestId)
                : new AppNotificationResponse(false, $"Push failed: {message}", requestId);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponse(false, $"Error processing response: {ex.Message}");
        }
    }

    private async Task<IEnumerable<AppNotificationResponse>> HandleBatchResponse(HttpResponseMessage response, string[] clientIds, CancellationToken ct)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);

            var code = doc.RootElement.GetProperty("code").GetInt32();
            var message = doc.RootElement.GetProperty("message").GetString() ?? string.Empty;
            var data = doc.RootElement.TryGetProperty("data", out var dataProp) ? dataProp : default;
            var requestId = (data.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : string.Empty) ?? string.Empty;
            var sendResult = data.TryGetProperty("sendResult", out var sendResultProp) && sendResultProp.GetBoolean();

            if (code == 80100000 && data.TryGetProperty("failTokens", out var failTokensProp))
            {
                var failTokens = failTokensProp.EnumerateArray().Select(token => token.GetString()).ToList();
                return clientIds.Select(x => new AppNotificationResponse(!failTokens.Contains(x), failTokens.Contains(x) ? "Failed token" : "Success", requestId, x));
            }

            return code == 200 && sendResult
                ? clientIds.Select(x => new AppNotificationResponse(true, "Success", requestId, x))
                : clientIds.Select(x => new AppNotificationResponse(false, $"Push failed: {message}", requestId, x));
        }
        catch (Exception ex)
        {
            return clientIds.Select(x => new AppNotificationResponse(false, $"Error processing response: {ex.Message}", string.Empty, x));
        }
    }

}
