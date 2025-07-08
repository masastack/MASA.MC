// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Getui;

public class GeTuiSenderProvider : IAppNotificationSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public AppPushProviders Provider => AppPushProviders.GeTui;

    public GeTuiSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<GetuiOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IGetuiOptions>>();
    }

    public IAppNotificationSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<GetuiSender>();
    }
}