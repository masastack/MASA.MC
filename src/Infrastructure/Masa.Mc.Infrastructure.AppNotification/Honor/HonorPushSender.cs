// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Masa.Mc.Infrastructure.AppNotification.Honor;

// 优化点：  
// 1. 减少重复代码：SendAsync 和 BatchSendAsync 方法中有大量重复逻辑，可以提取为一个通用方法。  
// 2. 提高代码可读性：将 BuildPayload 和 BuildClickAction 方法的逻辑进一步简化。  
// 3. 使用更明确的异常处理：在 HandleResponse 方法中增加对异常的处理。  

public class HonorPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly HonorAuthService _authService;
    private readonly IOptionsResolver<IHonorPushOptions> _optionsResolver;

    public HonorPushSender(HttpClient httpClient, HonorAuthService authService, IOptionsResolver<IHonorPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _authService = authService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        return await SendInternalAsync(appMessage, new[] { appMessage.ClientId }, ct);
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length > 1000)
            return new AppNotificationResponseBase(false, "Up to 1000 device tokens can be sent at a time");

        return await SendInternalAsync(appMessage, appMessage.ClientIds, ct);
    }

    public Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponseBase(false, "Honor Push does not support broadcast send"));

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponseBase(false, "Honor Push does not support message withdrawal"));

    private async Task<AppNotificationResponseBase> SendInternalAsync(AppMessage appMessage, string[] clientIds, CancellationToken ct)
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

        var response = await _httpClient.SendAsync(request, ct);
        return await HandleResponse(response, ct);
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
                : new { type = HonorClickActionType.AppDefinedIntent, action = url };
    }

    private async Task<AppNotificationResponseBase> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var json = await response.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(json);
            var code = doc.RootElement.GetProperty("code").GetInt32();
            var message = doc.RootElement.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : null;
            var data = doc.RootElement.TryGetProperty("data", out var dataProp) ? dataProp : default;
            var requestId = data.ValueKind != JsonValueKind.Undefined && data.TryGetProperty("requestId", out var idProp) ? idProp.GetString() : null;
            var sendResult = data.ValueKind != JsonValueKind.Undefined && data.TryGetProperty("sendResult", out var sendResultProp) ? sendResultProp.GetBoolean() : false;

            return code == 200 && sendResult
                ? new AppNotificationResponseBase(true, "Success", requestId)
                : new AppNotificationResponseBase(false, $"Push failed: {message}", requestId);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponseBase(false, $"Error processing response: {ex.Message}");
        }
    }
}
