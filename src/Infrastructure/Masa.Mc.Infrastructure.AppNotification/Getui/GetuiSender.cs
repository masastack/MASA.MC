// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Getui;

public class GetuiSender : IAppNotificationSender
{
    private readonly IOptionsResolver<IGetuiOptions> _optionsResolver;
    private static string HOST = "http://api.getui.com/apiex.htm";

    public GetuiSender(IOptionsResolver<IGetuiOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(HOST, options.AppKey, options.MasterSecret);
        NotificationTemplate template = NotificationTemplate(options, appMessage.Title, appMessage.Text, System.Text.Json.JsonSerializer.Serialize(appMessage.TransmissionContent));

        SingleMessage message = new SingleMessage();
        message.IsOffline = true;
        message.OfflineExpireTime = 1000 * 3600 * 12;
        message.Data = template;
        com.igetui.api.openservice.igetui.Target target = new com.igetui.api.openservice.igetui.Target();
        target.appId = options.AppID;
        target.clientId = appMessage.ClientId;

        try
        {
            var pushResult = push.pushMessageToSingle(message, target);

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("ok"))
            {
                return new AppNotificationResponseBase(false, pushResult);
            }

            return new AppNotificationResponseBase(true, "ok");
        }
        catch (RequestException e)
        {
            return new AppNotificationResponseBase(false, e.Message);
        }
    }

    public async Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(HOST, options.AppKey, options.MasterSecret);
        NotificationTemplate template = NotificationTemplate(options, appMessage.Title, appMessage.Text, System.Text.Json.JsonSerializer.Serialize(appMessage.TransmissionContent));

        com.igetui.api.openservice.igetui.AppMessage message = new com.igetui.api.openservice.igetui.AppMessage();
        message.IsOffline = true;
        message.OfflineExpireTime = 1000 * 3600 * 12;
        message.Data = template;
        var appIdList = new List<string>();
        appIdList.Add(options.AppID);
        message.AppIdList = appIdList;

        try
        {
            var pushResult = push.pushMessageToApp(message);

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("ok"))
            {
                return new AppNotificationResponseBase(false, pushResult);
            }

            return new AppNotificationResponseBase(true, "ok");
        }
        catch (RequestException e)
        {
            return new AppNotificationResponseBase(false, e.Message);
        }
    }

    public static NotificationTemplate NotificationTemplate(IGetuiOptions options, string title, string content, string transmissionContent)
    {
        NotificationTemplate template = new NotificationTemplate();
        template.AppId = options.AppID;
        template.AppKey = options.AppKey;
        template.Title = title;
        template.Text = content;
        template.Logo = "";
        template.LogoURL = "";
        template.TransmissionType = 1;
        template.TransmissionContent = transmissionContent;
        template.IsRing = true;
        template.IsVibrate = true;
        template.IsClearable = true;

        return template;
    }

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponseBase(false, "does not support message withdrawal"));
    }

    public Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponseBase(false, "does not support message batch send"));
    }
}
