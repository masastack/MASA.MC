// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Getui;

public class GetuiSender : IAppNotificationSender
{
    private readonly IOptionsResolver<IGetuiOptions> _optionsResolver;

    public bool SupportsBroadcast => true;

    public GetuiSender(IOptionsResolver<IGetuiOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(GetuiConstants.DomainUrl, options.AppKey, options.MasterSecret);
        var template = BuildNotificationTemplate(options, appMessage.Title, appMessage.Text, System.Text.Json.JsonSerializer.Serialize(appMessage.TransmissionContent));

        var message = new SingleMessage
        {
            IsOffline = true,
            OfflineExpireTime = 1000 * 3600 * 12,
            Data = template
        };
        var target = new com.igetui.api.openservice.igetui.Target
        {
            appId = options.AppID,
            clientId = appMessage.ClientId
        };

        try
        {
            var pushResult = push.pushMessageToSingle(message, target);

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("ok"))
                return new AppNotificationResponse(false, pushResult);

            return new AppNotificationResponse(true, "ok");
        }
        catch (RequestException e)
        {
            return new AppNotificationResponse(false, e.Message);
        }
    }

    public async Task<AppNotificationResponse> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(GetuiConstants.DomainUrl, options.AppKey, options.MasterSecret);
        var template = BuildNotificationTemplate(options, appMessage.Title, appMessage.Text, System.Text.Json.JsonSerializer.Serialize(appMessage.TransmissionContent));

        var listMessage = new ListMessage
        {
            IsOffline = true,
            OfflineExpireTime = 1000 * 3600 * 12,
            Data = template
        };

        string contentId;
        try
        {
            contentId = push.getContentId(listMessage, null);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponse(false, $"getContentId failed: {ex.Message}");
        }

        var targets = appMessage.ClientIds
            .Select(cid => new com.igetui.api.openservice.igetui.Target
            {
                appId = options.AppID,
                clientId = cid
            })
            .ToList();

        try
        {
            var pushResult = push.pushMessageToList(contentId, targets);

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("ok"))
                return new AppNotificationResponse(false, pushResult);

            return new AppNotificationResponse(true, "ok");
        }
        catch (RequestException e)
        {
            return new AppNotificationResponse(false, e.Message);
        }
    }

    public async Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(GetuiConstants.DomainUrl, options.AppKey, options.MasterSecret);
        var template = BuildNotificationTemplate(options, appMessage.Title, appMessage.Text, System.Text.Json.JsonSerializer.Serialize(appMessage.TransmissionContent));

        var message = new com.igetui.api.openservice.igetui.AppMessage
        {
            IsOffline = true,
            OfflineExpireTime = 1000 * 3600 * 12,
            Data = template,
            AppIdList = new List<string> { options.AppID }
        };

        try
        {
            var pushResult = push.pushMessageToApp(message);

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("ok"))
                return new AppNotificationResponse(false, pushResult);

            return new AppNotificationResponse(true, "ok");
        }
        catch (RequestException e)
        {
            return new AppNotificationResponse(false, e.Message);
        }
    }

    public static NotificationTemplate BuildNotificationTemplate(IGetuiOptions options, string title, string content, string transmissionContent)
    {
        return new NotificationTemplate
        {
            AppId = options.AppID,
            AppKey = options.AppKey,
            Title = title,
            Text = content,
            Logo = "",
            LogoURL = "",
            TransmissionType = 1,
            TransmissionContent = transmissionContent,
            IsRing = true,
            IsVibrate = true,
            IsClearable = true
        };
    }

    public Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support message withdrawal"));

    public Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default) 
        => Task.FromResult(new AppNotificationResponse(false, "does not support subscribe"));

    public Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support unsubscribe"));
}
