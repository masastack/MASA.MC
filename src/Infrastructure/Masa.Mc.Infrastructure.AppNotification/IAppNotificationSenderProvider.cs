// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public interface IAppNotificationSenderProvider
{
    Providers Provider { get; }

    IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties);
    IProviderAsyncLocalBase ResolveProviderAsyncLocal();
    IAppNotificationSender ResolveSender();
}