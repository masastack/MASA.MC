// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Honor;

public class HonorSenderProvider : IAppNotificationSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public Providers Provider => Providers.Honor;

    public HonorSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<HonorPushOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IHonorPushOptions>>();
    }

    public IAppNotificationSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<HonorPushSender>();
    }
}