// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class NotificationCommandHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public NotificationCommandHandler(IHubContext<NotificationsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [EventHandler]
    public async Task SendCheckNotificationAsync(SendCheckNotificationCommand command)
    {
        var singalRGroup = _hubContext.Clients.Group("Global");
        await singalRGroup.SendAsync(SignalRMethodConsts.CHECK_NOTIFICATION);
    }

    [EventHandler]
    public async Task SendGetNotificationAsync(SendGetNotificationCommand command)
    {
        var onlineClients = _hubContext.Clients.Users(command.UserIds);
        await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
    }
}
