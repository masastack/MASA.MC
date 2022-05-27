// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Service.Admin.Domain.Notifications;

namespace Masa.Mc.Service.Admin.Infrastructure.Notifications.Email;

public class EmailNotificationPublishProvider : NotificationPublishProvider
{
    public const string PROVIDER_NAME = NotificationProviderNames.EMAIL;
    public override string Name => PROVIDER_NAME;
}
