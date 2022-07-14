// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Notifications;

public class NotificationCommandHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;

    public NotificationCommandHandler(IHubContext<NotificationsHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [EventHandler]
    public async Task SendNotificationAsync(SendNotificationCommand command)
    {
        if (!string.IsNullOrWhiteSpace(command.dto.GroupId))
        {
            var singalRGroup = _hubContext.Clients.Group(command.dto.GroupId);
            await singalRGroup.SendAsync(command.dto.MethodName);
        }
        else
        {
            var onlineClients = _hubContext.Clients.Users(command.dto.UserIds);
            await onlineClients.SendAsync(command.dto.MethodName);
        }
    }
}
