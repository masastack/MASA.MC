// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Vivo;

public class VivoPushSender : IAppNotificationSender
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsResolver<IVivoPushOptions> _optionsResolver;
    private readonly VivoAuthService _authService;

    public bool SupportsBroadcast => true;

    public bool SupportsReceipt => true;

    public VivoPushSender(HttpClient httpClient, IOptionsResolver<IVivoPushOptions> optionsResolver, VivoAuthService authService)
    {
        _httpClient = httpClient;
        _optionsResolver = optionsResolver;
        _authService = authService;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await CreatePayloadAsync(appMessage, ct);
        return await SendWithResolvedOptionsAsync(VivoConstants.SendUrl, payload, ct);
    }

    public async Task<IEnumerable<AppNotificationResponse>> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        if (appMessage.ClientIds.Length < 2 || appMessage.ClientIds.Length > 1000)
            return appMessage.ClientIds.Select(x => new AppNotificationResponse(false, "regIds count must be 2~1000", string.Empty, x));

        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options, ct);

        var savePayload = await CreatePayloadAsync(appMessage, ct);
        var saveResponse = await SendRequestAsync(VivoConstants.SaveListPayloadUrl, savePayload, token, ct);
        var saveResult = await HandleResponse(saveResponse, ct);
        if (!saveResult.Success) 
            return appMessage.ClientIds.Select(x => new AppNotificationResponse(saveResult.Success, saveResult.Message, saveResult.MsgId, x));

        var pushPayload = await CreatePayloadAsync(appMessage, ct, null, appMessage.ClientIds, saveResult.MsgId);

        var response = await SendRequestAsync(VivoConstants.PushToListUrl, pushPayload, token, ct);
        return await HandleBatchResponse(response, appMessage.ClientIds, saveResult.MsgId, ct);
    }

    public async Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var payload = await CreatePayloadAsync(appMessage, ct, new { orTags = new[] { AppNotificationConstants.BroadcastTag } });
        return await SendWithResolvedOptionsAsync(VivoConstants.TagPushUrl, payload, ct);
    }

    public async Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        var payload = new
        {
            appId = (await _optionsResolver.ResolveAsync()).AppId,
            taskId = msgId,
            userIds = Array.Empty<string>(),
            userType = 1
        };
        return await SendWithResolvedOptionsAsync(VivoConstants.RecycleUrl, payload, ct);
    }

    public async Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(clientId))
            return new AppNotificationResponse(false, "regId is required");

        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options, ct);

        var payload = new
        {
            appId = options.AppId,
            name = name,
            type = 1,
            ids = new[] { clientId }
        };

        var response = await SendRequestAsync(VivoConstants.AddMembersUrl, payload, token, ct);
        return await HandleResponse(response, ct);
    }

    public async Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(clientId))
            return new AppNotificationResponse(false, "regId is required");

        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options, ct);

        var payload = new
        {
            appId = options.AppId,
            name = name,
            type = 1,
            ids = new[] { clientId }
        };

        var response = await SendRequestAsync(VivoConstants.RemoveMembersUrl, payload, token, ct);
        return await HandleResponse(response, ct);
    }

    private async Task<AppNotificationResponse> SendWithResolvedOptionsAsync(string url, object payload, CancellationToken ct)
    {
        var options = await _optionsResolver.ResolveAsync();
        var token = await _authService.GetAccessTokenAsync(options, ct);
        var response = await SendRequestAsync(url, payload, token, ct);
        return await HandleResponse(response, ct);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(string url, object payload, string? token = null, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(payload)
        };
        if (!string.IsNullOrEmpty(token))
            request.Headers.Add("authToken", token);

        return await _httpClient.SendAsync(request, ct);
    }

    private int BuildSkipType(string url)
    {
        if (string.IsNullOrEmpty(url))
            return 1;
        if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return 2;
        return 4;
    }

    private async Task<object> CreatePayloadAsync(AppMessage appMessage, CancellationToken ct, object? tagExpression = null, string[]? regIds = null, string? taskId = null)
    {
        var options = await _optionsResolver.ResolveAsync();

        return new
        {
            appId = options.AppId,
            regId = appMessage is SingleAppMessage single ? single.ClientId : null,
            regIds,
            tagExpression,
            taskId,
            notifyType = 4,
            title = appMessage.Title,
            content = appMessage.Text,
            skipType = BuildSkipType(appMessage.Url),
            skipContent = appMessage.Url,
            requestId = Guid.NewGuid().ToString("N"),
            extra = new Dictionary<string, string>
           {
               { "callback.id", options.CallbackId }
           }
        };
    }

    private async Task<AppNotificationResponse> HandleResponse(HttpResponseMessage response, CancellationToken ct)
    {
        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement.GetProperty("result").GetInt32();
        var desc = doc.RootElement.GetProperty("desc").GetString() ?? "";
        var taskId = doc.RootElement.TryGetProperty("taskId", out var idProp) ? idProp.GetString() ?? "" : "";

        return new AppNotificationResponse(result == 0, desc, taskId);
    }

    private async Task<IEnumerable<AppNotificationResponse>> HandleBatchResponse(HttpResponseMessage response, string[] clientIds, string msgId, CancellationToken ct)
    {
        var json = await response.Content.ReadAsStringAsync(ct);
        using var doc = JsonDocument.Parse(json);
        var result = doc.RootElement.GetProperty("result").GetInt32();
        var desc = doc.RootElement.GetProperty("desc").GetString() ?? "";

        var errorTokens = new List<string>();
        if (doc.RootElement.TryGetProperty("invalidUser", out var invalidUserProp) && invalidUserProp.ValueKind == JsonValueKind.Object)
        {
            if (invalidUserProp.TryGetProperty("userid", out var userIdProp) && userIdProp.ValueKind == JsonValueKind.String)
                errorTokens.Add(userIdProp.GetString() ?? "");
        }

        if (result == 0)
        {
            return clientIds.Select(x => new AppNotificationResponse(true, desc, msgId, x));
        }
        else if (errorTokens.Count > 0)
        {
            return clientIds.Select(x => new AppNotificationResponse(!errorTokens.Contains(x), errorTokens.Contains(x) ? "Error token" : "Success", msgId, x));
        }
        else
        {
            return clientIds.Select(x => new AppNotificationResponse(false, desc, msgId, x));
        }
    }
}
