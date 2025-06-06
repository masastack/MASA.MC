// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public class XiaomiSenderProvider : IAppNotificationSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public Providers Provider => Providers.Xiaomi;

    public XiaomiSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<XiaomiPushOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IXiaomiPushOptions>>();
    }

    public IAppNotificationSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<XiaomiPushSender>();
    }
}