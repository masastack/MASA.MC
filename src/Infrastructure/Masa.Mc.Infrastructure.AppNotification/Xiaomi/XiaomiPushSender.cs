namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public class XiaomiPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsResolver<IXiaomiPushOptions> _optionsResolver;

    public bool SupportsBroadcast => true;

    public bool SupportsReceipt => true;

    public XiaomiPushSender(HttpClient httpClient, IOptionsResolver<IXiaomiPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, appMessage.ClientId, ct);
        return await SendPushAsync(XiaomiConstants.SendToRegIdUrl, payload, ct);
    }

    public async Task<IEnumerable<AppNotificationResponse>> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length > 1000)
            return appMessage.ClientIds.Select(x => new AppNotificationResponse(false, "Up to 1000 device tokens can be sent at a time", string.Empty, x));

        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, string.Join(",", appMessage.ClientIds), ct);
        var response = await SendPushAsync(XiaomiConstants.SendToRegIdUrl, payload, ct);
        return appMessage.ClientIds.Select(x => new AppNotificationResponse(response.Success, response.Message, response.RegId, x));
    }

    public async Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, null, ct);
        return await SendPushAsync(XiaomiConstants.SendToAllUrl, payload, ct);
    }

    public async Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
           {
               { "restricted_package_name", options.PackageName },
               { "msg_id", msgId }
           };
        return await SendPushAsync(XiaomiConstants.RevokeUrl, payload, ct);
    }

    public async Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
        {
            { "registration_id", clientId },
            { "topic", name },
            { "restricted_package_name", options.PackageName }
        };

        var response = await SendPushAsync(XiaomiConstants.TopicSubscribeUrl, payload, ct);
        response.Message = response.Success
            ? "Subscribe topic succeeded"
            : $"Subscribe topic failed: {response.Message}";
        return response;
    }

    public async Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
        {
            { "registration_id", clientId },
            { "topic", name },
            { "restricted_package_name", options.PackageName }
        };

        var response = await SendPushAsync(XiaomiConstants.TopicUnsubscribeUrl, payload, ct);
        response.Message = response.Success
            ? "Unsubscribe topic succeeded"
            : $"Unsubscribe topic failed: {response.Message}";
        return response;
    }

    private async Task<Dictionary<string, string>> BuildPayloadAsync(string title, string description, ConcurrentDictionary<string, object> transmissionContent, string url, string? clientId, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
           {
               { "title", title },
               { "description", description },
               { "payload", System.Text.Json.JsonSerializer.Serialize(transmissionContent ?? new()) },
               { "restricted_package_name", options.PackageName },
               { "extra.callback", options.CallbackUrl},
               { "extra.callback.type", ((int)(XiaomiCallbackType.Delivered
               | XiaomiCallbackType.InvalidDevice
               | XiaomiCallbackType.PushDisabled
               | XiaomiCallbackType.FilterMismatch
               | XiaomiCallbackType.PushLimitExceeded
               | XiaomiCallbackType.TTLEnded)).ToString() }
           };

        if (!string.IsNullOrEmpty(clientId))
        {
            payload["registration_id"] = clientId;
        }

        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            payload["extra.notify_effect"] = "3";
            payload["extra.web_uri"] = url;
        }
        else if (!string.IsNullOrEmpty(url))
        {
            payload["extra.notify_effect"] = "2";
            payload["extra.intent_uri"] = url;
        }
        else
        {
            payload["extra.notify_effect"] = "1";
        }

        return payload;
    }

    private async Task<AppNotificationResponse> SendPushAsync(string url, Dictionary<string, string> payload, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(payload)
        };
        request.Headers.TryAddWithoutValidation("Authorization", $"key={options.AppSecret}");

        try
        {
            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new AppNotificationResponse(false, $"Push failed with status code: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            var code = root.GetProperty("code").GetInt32();
            var msgId = root.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
            var result = root.GetProperty("result").GetString() ?? "";
            var desc = root.GetProperty("description").GetString() ?? "";

            return code == 0 && result == "ok"
                ? new AppNotificationResponse(true, "Push succeeded", msgId)
                : new AppNotificationResponse(false, desc, msgId);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponse(false, $"Push exception: {ex.Message}");
        }
    }
}
