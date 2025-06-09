// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public class OppoPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly OppoAuthService _authService;
    private readonly IOptionsResolver<IOppoPushOptions> _optionsResolver;

    public OppoPushSender(HttpClient httpClient, OppoAuthService authService, IOptionsResolver<IOppoPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _authService = authService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options.AppKey, options.MasterSecret, ct);

        var messageObj = new
        {
            target_type = OppoTargetType.RegistrationId,
            target_value = appMessage.ClientId,
            verify_registration_id = true,
            notification = BuildNotification(appMessage)
        };

        var form = new Dictionary<string, string>
        {
            { "auth_token", token },
            { "message", JsonSerializer.Serialize(messageObj) }
        };

        var content = new FormUrlEncodedContent(form);
        var response = await _httpClient.PostAsync(OppoConstants.UnicastUrl, content, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length > 1000)
            return new AppNotificationResponseBase(false, "Up to 1000 device tokens can be sent at a time");

        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options.AppKey, options.MasterSecret, ct);

        var messages = appMessage.ClientIds.Select(id => new
        {
            target_type = OppoTargetType.RegistrationId,
            target_value = id,
            notification = BuildNotification(appMessage)
        }).ToArray();

        var form = new Dictionary<string, string>
        {
            { "auth_token", token },
            { "messages", JsonSerializer.Serialize(messages) }
        };

        var content = new FormUrlEncodedContent(form);
        var response = await _httpClient.PostAsync(OppoConstants.UnicastBatchUrl, content, ct);
        return await HandleResponse(response, ct);
    }

    private Dictionary<string, object?> BuildNotification(AppMessage appMessage)
    {
        var notification = new Dictionary<string, object?>
        {
            { "app_message_id", Guid.NewGuid().ToString("N") },
            { "title", appMessage.Title },
            { "content", appMessage.Text },
            { "off_line_ttl", 86400 }
        };

        if (appMessage.Url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            notification["click_action_type"] = 2;
            notification["click_action_url"] = appMessage.Url;
        }
        else if(!string.IsNullOrEmpty(appMessage.Url))
        {
            notification["click_action_type"] = 1;
            notification["click_action_activity"] = appMessage.Url;
        }

        return notification;
    }

    private async Task<string> SaveBroadcastMessageAsync(AppMessage appMessage, string authToken, CancellationToken ct)
    {
        var notification = BuildNotification(appMessage);
        var form = notification.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString() ?? string.Empty);
        form["auth_token"] = authToken;

        var content = new FormUrlEncodedContent(form);
        var response = await _httpClient.PostAsync(OppoConstants.SaveMessageContentUrl, content, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("data").GetProperty("message_id").GetString()!;
    }

    public async Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options.AppKey, options.MasterSecret, ct);
        var messageId = await SaveBroadcastMessageAsync(appMessage, token, ct);

        var form = new Dictionary<string, string>
        {
            { "auth_token", token },
            { "message_id", messageId },
            { "target_type", ((int)OppoTargetType.Tag).ToString() },
            { "target_value", AppNotificationConstants.BroadcastTag }
        };
        var content = new FormUrlEncodedContent(form);
        var response = await _httpClient.PostAsync(OppoConstants.BroadcastUrl, content, ct);
        return await HandleResponse(response, ct);
    }

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponseBase(false, "OPPO Push does not support message withdrawal"));

    private async Task<AppNotificationResponseBase> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        var json = await response.Content.ReadAsStringAsync(ct);
        try
        {
            using var doc = JsonDocument.Parse(json);
            var code = doc.RootElement.GetProperty("code").GetInt32();
            var message = doc.RootElement.GetProperty("message").GetString() ?? "";
            var msgId = doc.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("message_id", out var idProp) ? idProp.GetString() ?? "" : "";

            if (code == 0)
                return new AppNotificationResponseBase(true, "Push succeeded", msgId);
            else
                return new AppNotificationResponseBase(false, $"Push failed: {message}");
        }
        catch
        {
            return new AppNotificationResponseBase(false, $"Push failed: {json}");
        }
    }
}
