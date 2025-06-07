namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public class XiaomiPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsResolver<IXiaomiPushOptions> _optionsResolver;

    public XiaomiPushSender(HttpClient httpClient, IOptionsResolver<IXiaomiPushOptions> optionsResolver)
    {
        _httpClient = httpClient;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, appMessage.ClientId, ct);
        return await SendPushAsync(XiaomiConstants.SendToRegIdUrl, payload, ct);
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length > 1000)
            return new AppNotificationResponseBase(false, "Up to 1000 device tokens can be sent at a time");

        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, string.Join(",", appMessage.ClientIds), ct);
        return await SendPushAsync(XiaomiConstants.SendToRegIdUrl, payload, ct);
    }

    public async Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await BuildPayloadAsync(appMessage.Title, appMessage.Text, appMessage.TransmissionContent, appMessage.Url, null, ct);
        return await SendPushAsync(XiaomiConstants.SendToAllUrl, payload, ct);
    }

    public async Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
           {
               { "restricted_package_name", options.PackageName },
               { "msg_id", msgId }
           };
        return await SendPushAsync(XiaomiConstants.RevokeUrl, payload, ct);
    }

    private async Task<Dictionary<string, string>> BuildPayloadAsync(string title, string description, ConcurrentDictionary<string, object> transmissionContent, string url, string? clientId, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();

        var payload = new Dictionary<string, string>
           {
               { "title", title },
               { "description", description },
               { "payload", System.Text.Json.JsonSerializer.Serialize(transmissionContent ?? new()) },
               { "restricted_package_name", options.PackageName }
           };

        if (!string.IsNullOrEmpty(clientId))
        {
            payload["registration_id"] = clientId;
        }

        if (!string.IsNullOrEmpty(url))
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

    private async Task<AppNotificationResponseBase> SendPushAsync(string url, Dictionary<string, string> payload, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(payload)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("key", options.AppSecret);

        try
        {
            var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                return new AppNotificationResponseBase(false, $"Push failed with status code: {response.StatusCode}");
            }

            var responseString = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(responseString);
            var root = doc.RootElement;
            var code = root.GetProperty("code").GetInt32();
            var msgId = root.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idProp) ? idProp.GetString() ?? "" : "";
            var result = root.GetProperty("result").GetString() ?? "";
            var desc = root.GetProperty("description").GetString() ?? "";

            return code == 0 && result == "ok"
                ? new AppNotificationResponseBase(true, "Push succeeded", msgId)
                : new AppNotificationResponseBase(false, desc);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponseBase(false, $"Push exception: {ex.Message}");
        }
    }
}
