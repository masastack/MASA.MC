// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Huawei;

public class HuaweiSenderProvider : IAppNotificationSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public Providers Provider => Providers.HuaweiPush;

    public HuaweiSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<HuaweiPushOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IHuaweiPushOptions>>();
    }

    public IAppNotificationSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<HuaweiPushSender>();
    }
}