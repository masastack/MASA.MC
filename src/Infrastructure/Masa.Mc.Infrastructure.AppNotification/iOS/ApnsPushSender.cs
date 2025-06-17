// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using dotAPNS;
using dotAPNS.AspNetCore;

namespace Masa.Mc.Infrastructure.AppNotification.iOS;

public class ApnsPushSender : IAppNotificationSender
{
    private readonly IApnsService _apnsService;
    private readonly IOptionsResolver<IiOSPushOptions> _optionsResolver;

    public bool SupportsBroadcast => false;

    public ApnsPushSender(IApnsService apnsService, IOptionsResolver<IiOSPushOptions> optionsResolver)
    {
        _apnsService = apnsService;
        _optionsResolver = optionsResolver;
    }

    public async Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default)
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
            return new AppNotificationResponse(false, $"Push failed due to an exception: {ex.Message}");
        }
    }

    public async Task<AppNotificationResponse> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default)
    {
        try
        {
            var options = await _optionsResolver.ResolveAsync();
            var pushes = appMessage.ClientIds.Select(clientId =>
                CreatePush(appMessage.Title, appMessage.Text, clientId, appMessage.TransmissionContent)).ToList();

            var apnsJwtOptions = options as ApnsJwtOptions;
            var responses = await _apnsService.SendPushes(pushes, apnsJwtOptions);

            var results = responses.Select((response, index) => new
            {
                Response = CreateResponse(response),
                Token = appMessage.ClientIds[index]
            }).ToList();

            var failedTokens = results.Where(r => !r.Response.Success).Select(r => r.Token).ToList();
            var successCount = results.Count(r => r.Response.Success);

            string message;
            bool success;
            if (successCount == results.Count)
            {
                message = "Batch push succeeded";
                success = true;
            }
            else if (successCount > 0)
            {
                message = "Partial push success";
                success = true;
            }
            else
            {
                message = string.Join(";", results.Select(r => r.Response.Message));
                success = false;
            }

            return new AppNotificationResponse(success, message, errorTokens: failedTokens);
        }
        catch (Exception ex)
        {
            return new AppNotificationResponse(false, $"Batch push failed due to an exception: {ex.Message}");
        }
    }


    public Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponse(false, "APNs does not support broadcast push"));
    }

    public Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponse(false, "APNs does not support message withdrawal"));
    }

    public Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponse(false, "APNs does not support tag subscribe"));
    }

    public Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default)
    {
        return Task.FromResult(new AppNotificationResponse(false, "APNs does not support tag unsubscribe"));
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

    private AppNotificationResponse CreateResponse(ApnsResponse response)
    {
        return response.IsSuccessful
            ? new AppNotificationResponse(true, "Push succeeded")
            : new AppNotificationResponse(false, $"Push failed: {response.Reason}");
    }
}
