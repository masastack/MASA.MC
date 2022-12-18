// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Getui;

public class GetuiSender : IAppNotificationSender
{
    private readonly IAppNotificationOptionsResolver _getuiOptionsResolver;
    private static string HOST = "http://api.getui.com/apiex.htm";

    public GetuiSender(IAppNotificationOptionsResolver getuiOptionsResolver)
    {
        _getuiOptionsResolver = getuiOptionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(AppMessage appMessage)
    {
        var options = await _getuiOptionsResolver.ResolveAsync();
        IGtPush push = new IGtPush(HOST, options.AppKey, options.MasterSecret);
        NotificationTemplate template = NotificationTemplate(options, appMessage.Title, appMessage.Text, appMessage.TransmissionContent);

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

            if (string.IsNullOrWhiteSpace(pushResult) || !pushResult.Contains("successed"))
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

    public static NotificationTemplate NotificationTemplate(IAppNotificationOptions options, string title, string content, string transmissionContent)
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
}
