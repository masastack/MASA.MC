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
                    Alert = new NotificationIOSAlert
                    {
                        Title = appMessage.Title,
                        Body = appMessage.Text
                    },
                    Badge = "+1",
                    Extras = appMessage.TransmissionContent.ToDictionary(x => x.Key, x => x.Value)
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
                IsApnsProduction = appMessage.IsApnsProduction
            }
        };

        try
        {
            var response = client.SendPush(pushPayload);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new AppNotificationResponseBase(false, response.Content);
            }

            var successResponse = JsonConvert.DeserializeObject<SendPushSuccessResponse>(response.Content);

            return new AppNotificationResponseBase(true, "ok", successResponse.MsgId);
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
                    Alert = new NotificationIOSAlert
                    {
                        Title = appMessage.Title,
                        Body = appMessage.Text
                    },
                    Badge = "+1",
                    Extras = appMessage.TransmissionContent.ToDictionary(x => x.Key, x => x.Value)
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
                IsApnsProduction = appMessage.IsApnsProduction
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

    public async Task<AppNotificationResponseBase> WithdrawnAsync(string msgId)
    {
        var options = await _jPushOptionsResolver.ResolveAsync();
        JPushClient client = new JPushClient(options.AppKey, options.MasterSecret);

        HttpResponseMessage response = await JPushClient.HttpClient.DeleteAsync($"{JPushClient.BASE_URL_PUSH_DEFAULT}/{msgId}");
        var content = await response.Content.ReadAsStringAsync();

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return new AppNotificationResponseBase(false, content);
        }

        return new AppNotificationResponseBase(true, "ok");
    }
}
