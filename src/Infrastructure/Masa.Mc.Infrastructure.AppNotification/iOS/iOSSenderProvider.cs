// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.iOS;

public class iOSSenderProvider : IAppNotificationSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public AppPushProviders Provider => AppPushProviders.iOS;

    public iOSSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<iOSPushOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IiOSPushOptions>>();
    }

    public IAppNotificationSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<ApnsPushSender>();
    }
}