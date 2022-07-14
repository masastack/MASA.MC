// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Notifications.SignalR.Hubs;

[Authorize]
public class NotificationsHub: Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Groups.AddToGroupAsync(Context.ConnectionId, "Global", Context.ConnectionAborted);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Global", Context.ConnectionAborted);
    }
}
