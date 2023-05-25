// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public class JPushSender : IAppNotificationSender
{
    private readonly IAppNotificationOptionsResolver _jPushOptionsResolver;

    public JPushSender(IAppNotificationOptionsResolver jPushOptionsResolver)
    {
        _jPushOptionsResolver = jPushOptionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage)
    {
        var options = await _jPushOptionsResolver.ResolveAsync();
        JPushClient client = new JPushClient(options.AppKey, options.MasterSecret);

        var audience = new
        {
            registration_id = new string[] { appMessage.ClientId }
        };

        PushPayload pushPayload = new PushPayload()
        {
            Platform = new List<string> { "android", "ios" },
            Audience = audience,
            Notification = new Notification
            {
                Alert = appMessage.Title,
                Android = new Android
                {
                    Alert = appMessage.Text,
                    Title = appMessage.Title,
                    Indent = new Dictionary<string, object>
                    {
                        ["url"] = appMessage.Url ?? string.Empty
                    }
                },
                IOS = new IOS
                {
                    Alert = new
                    {
                        title = appMessage.Title,
                        body = appMessage.Text
                    },
                    Badge = "+1"
                }
            },
            Message = new Jiguang.JPush.Model.Message
            {
                Title = appMessage.Title,
                Content = appMessage.Text,
                Extras = appMessage.TransmissionContent
            },
            Options = new Jiguang.JPush.Model.Options
            {
                IsApnsProduction = true // 设置 iOS 推送生产环境。不设置默认为开发环境。
            }
        };

        try
        {
            var response = client.SendPush(pushPayload);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new AppNotificationResponseBase(false, response.Content);
            }

            return new AppNotificationResponseBase(true, "ok");
        }
        catch (Exception e)
        {
            return new AppNotificationResponseBase(false, e.Message);
        }
    }

    public async Task<AppNotificationResponseBase> SendAllAsync(AppMessage appMessage)
    {
        var options = await _jPushOptionsResolver.ResolveAsync();
        JPushClient client = new JPushClient(options.AppKey, options.MasterSecret);

        PushPayload pushPayload = new PushPayload()
        {
            Platform = new List<string> { "android", "ios" },
            Audience = "all",
            Notification = new Notification
            {
                Alert = appMessage.Title,
                Android = new Android
                {
                    Alert = appMessage.Title,
                    Indent = new Dictionary<string, object>
                    {
                        ["url"] = appMessage.Url ?? string.Empty
                    }
                },
                IOS = new IOS
                {
                    Alert = new
                    {
                        title = appMessage.Title,
                        body = appMessage.Text
                    },
                    Badge = "+1"
                }
            },
            Message = new Jiguang.JPush.Model.Message
            {
                Title = appMessage.Title,
                Content = appMessage.Text,
                Extras = appMessage.TransmissionContent
            },
            Options = new Jiguang.JPush.Model.Options
            {
                IsApnsProduction = true // 设置 iOS 推送生产环境。不设置默认为开发环境。
            }
        };

        try
        {
            var response = client.SendPush(pushPayload);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new AppNotificationResponseBase(false, response.Content);
            }

            return new AppNotificationResponseBase(true, "ok");
        }
        catch (Exception e)
        {
            return new AppNotificationResponseBase(false, e.Message);
        }
    }
}
