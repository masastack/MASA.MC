// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR;

public class NotificationsSignalROptions
{
    public string MethodName { get; set; }

    public NotificationsSignalROptions()
    {
        MethodName = "get-notification";
    }
}
