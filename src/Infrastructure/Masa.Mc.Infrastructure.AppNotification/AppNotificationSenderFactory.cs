// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification;

public class AppNotificationSenderFactory
{
    private readonly Dictionary<AppPushProviders, IAppNotificationSenderProvider> _providers;

    public AppNotificationSenderFactory(IEnumerable<IAppNotificationSenderProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.Provider);
    }

    public IOptions GetOptions(AppPushProviders provider, ConcurrentDictionary<string, object> extraProperties)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveOptions(extraProperties);
        }

        throw new KeyNotFoundException($"No options resolver found for '{provider}'");
    }

    public IProviderAsyncLocalBase GetProviderAsyncLocal(AppPushProviders provider)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveProviderAsyncLocal();
        }

        throw new KeyNotFoundException($"No async local resolver found for '{provider}'");
    }

    public IAppNotificationSender GetAppNotificationSender(AppPushProviders provider)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveSender();
        }

        throw new KeyNotFoundException($"No sender resolver found for '{provider}'");
    }
}