// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Notifications;

public abstract class NotificationPublishProvider : INotificationPublishProvider
{
    public abstract string Name { get; }

    public Task PublishAsync()
    {
        throw new NotImplementedException();
    }
}
