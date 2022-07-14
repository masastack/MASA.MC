// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class NotificationService : ServiceBase
{
    public NotificationService(IServiceCollection services) : base(services, "api/notification")
    {
        MapPost(SendNotificationAsync);
    }

    public async Task SendNotificationAsync(IEventBus eventBus, SendNotificationDto dto)
    {
        var command = new SendNotificationCommand(dto);
        await eventBus.PublishAsync(command);
    }
}
