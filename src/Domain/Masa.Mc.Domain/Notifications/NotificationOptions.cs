// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Notifications;

public class NotificationOptions
{
    public IList<INotificationPublishProvider> PublishProviders { get; }

    public NotificationOptions()
    {
        PublishProviders = new List<INotificationPublishProvider>();
    }
}
