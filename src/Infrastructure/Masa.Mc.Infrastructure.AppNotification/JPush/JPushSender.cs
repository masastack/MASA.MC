// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public class JPushSender : IAppNotificationSender
{
    private readonly IOptionsResolver<IJPushOptions> _optionsResolver;

    public bool SupportsBroadcast => true;

    public JPushSender(IOptionsResolver<IJPushOptions> optionsResolver)
    {
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var client = new JPushClient(options.AppKey, options.MasterSecret);

        var audience = new { registration_id = new[] { appMessage.ClientId } };
        var pushPayload = BuildPushPayload(appMessage, audience);

        return await SendPushAsync(client, pushPayload);
    }

    public async Task<AppNotificationResponse> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var client = new JPushClient(options.AppKey, options.MasterSecret);

        var audience = new { registration_id = appMessage.ClientIds };
        var pushPayload = BuildPushPayload(appMessage, audience);

        return await SendPushAsync(client, pushPayload);
    }

    public async Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var client = new JPushClient(options.AppKey, options.MasterSecret);

        var pushPayload = BuildPushPayload(appMessage, AppNotificationConstants.BroadcastTag);

        return await SendPushAsync(client, pushPayload, isBroadcast: true);
    }

    public async Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        var options = await _optionsResolver.ResolveAsync();
        var client = new JPushClient(options.AppKey, options.MasterSecret);

        var response = await JPushClient.HttpClient.DeleteAsync($"{JPushClient.BASE_URL_PUSH_DEFAULT}/{msgId}");
        var content = await response.Content.ReadAsStringAsync();

        return response.StatusCode == HttpStatusCode.OK
            ? new AppNotificationResponse(true, "ok")
            : new AppNotificationResponse(false, content);
    }

    public Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support subscribe"));

    public Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
        => Task.FromResult(new AppNotificationResponse(false, "does not support unsubscribe"));

    private async Task<AppNotificationResponse> SendPushAsync(JPushClient client, PushPayload pushPayload, bool isBroadcast = false)
    {
        try
        {
            var response = client.SendPush(pushPayload);

            if (response.StatusCode != HttpStatusCode.OK)
                return new AppNotificationResponse(false, response.Content);

            if (isBroadcast)
                return new AppNotificationResponse(true, "ok");

            var successResponse = JsonConvert.DeserializeObject<SendPushSuccessResponse>(response.Content);
            return new AppNotificationResponse(true, "ok", successResponse?.MsgId);
        }
        catch (Exception e)
        {
            return new AppNotificationResponse(false, e.Message);
        }
    }

    private PushPayload BuildPushPayload(AppMessage appMessage, object audience)
    {
        return new PushPayload
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
    }
}
