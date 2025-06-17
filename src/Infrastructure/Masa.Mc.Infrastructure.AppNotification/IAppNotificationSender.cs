// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public interface IAppNotificationSender : ITransientDependency
{
    bool SupportsBroadcast {  get; }

    Task<AppNotificationResponse> SendAsync(SingleAppMessage appMessage, CancellationToken ct = default);

    Task<AppNotificationResponse> BatchSendAsync(BatchAppMessage appMessage, CancellationToken ct = default);

    Task<AppNotificationResponse> BroadcastSendAsync(AppMessage appMessage, CancellationToken ct = default);

    Task<AppNotificationResponse> WithdrawnAsync(string msgId, CancellationToken ct = default);

    Task<AppNotificationResponse> SubscribeAsync(string name, string clientId, CancellationToken ct = default);

    Task<AppNotificationResponse> UnsubscribeAsync(string name, string clientId, CancellationToken ct = default);
}
