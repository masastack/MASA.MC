// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Service.Admin.Domain.Notifications;

namespace Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR;

public class SignalRNotificationPublishProvider : NotificationPublishProvider
{
    public const string PROVIDER_NAME = NotificationProviderNames.SIGNAL_R;
    public override string Name => PROVIDER_NAME;

    private readonly IHubContext<NotificationsHub> _hubContext;
}
