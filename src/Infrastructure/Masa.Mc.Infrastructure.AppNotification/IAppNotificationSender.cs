// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public interface IAppNotificationSender
{
    Task<AppNotificationResponseBase> SendAsync(SingleAppMessage appMessage);

    Task<AppNotificationResponseBase> SendAllAsync(AppMessage appMessage);
}
