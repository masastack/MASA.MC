// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using dotAPNS;
using dotAPNS.AspNetCore;

namespace Masa.Mc.Infrastructure.AppNotification.iOS;

public class ApnsPushSender : IAppNotificationSender
{
    private readonly IApnsService _apnsService;
    private readonly IOptionsResolver<IiOSPushOptions> _optionsResolver;

    public ApnsPushSender(IApnsService apnsService, IOptionsResolver<IiOSPushOptions> optionsResolver)
    {
        _apnsService = apnsService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
    {
        try
        {
            var options = await _optionsResolver.ResolveAsync();
            var push = CreatePush(appMessage.Title, appMessage.Text, appMessage.ClientId, appMessage.TransmissionContent);
            var apnsJwtOptions = options as ApnsJwtOptions;

            var response = await _apnsService.SendPush(push, apnsJwtOptions);
            return CreateResponse(response);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponseBase(false, $"Push failed due to an exception: {ex.Message}");
        }
    }

    public async Task<AppNotificationResponseBase> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        try
        {
            var options = await _optionsResolver.ResolveAsync();
            var pushes = appMessage.ClientIds.Select(clientId =>
                CreatePush(appMessage.Title, appMessage.Text, clientId, appMessage.TransmissionContent)).ToList();

            var apnsJwtOptions = options as ApnsJwtOptions;
            var responses = await _apnsService.SendPushes(pushes, apnsJwtOptions);

            var results = responses.Select(CreateResponse).ToList();
            var success = results.All(r => r.Success);
            var message = success ? "Batch push succeeded" : string.Join(";", results.Where(r => !r.Success).Select(r => r.Message));
            return new AppNotificationResponseBase(success, message);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponseBase(false, $"Batch push failed due to an exception: {ex.Message}");
        }
    }

    public Task<AppNotificationResponseBase> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponseBase(false, "APNs does not support broadcast push"));
    }

    public Task<AppNotificationResponseBase> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponseBase(false, "APNs does not support message withdrawal"));
    }

    private ApplePush CreatePush(string title, string text, string clientId, ConcurrentDictionary<string, object> transmissionContent)
    {
        var push = new ApplePush(ApplePushType.Alert)
            .AddAlert(title, text)
            .AddToken(clientId);

        if (transmissionContent?.Any() == true)
        {
            foreach (var kvp in transmissionContent)
            {
                push.AddCustomProperty(kvp.Key, kvp.Value);
            }
        }

        return push;
    }

    private AppNotificationResponseBase CreateResponse(ApnsResponse response)
    {
        return response.IsSuccessful
            ? new AppNotificationResponseBase(true, "Push succeeded")
            : new AppNotificationResponseBase(false, $"Push failed: {response.Reason}");
    }
}
