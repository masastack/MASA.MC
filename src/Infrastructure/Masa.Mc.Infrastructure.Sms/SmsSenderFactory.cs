// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms;

public class SmsSenderFactory
{
    private readonly Dictionary<SmsProviders, ISmsSenderProvider> _providers;

    public SmsSenderFactory(IEnumerable<ISmsSenderProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.Provider);
    }

    public IOptions GetOptions(SmsProviders provider, ConcurrentDictionary<string, object> extraProperties)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveOptions(extraProperties);
        }

        throw new KeyNotFoundException($"No options resolver found for '{provider}'");
    }

    public IProviderAsyncLocalBase GetProviderAsyncLocal(SmsProviders provider)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveProviderAsyncLocal();
        }

        throw new KeyNotFoundException($"No async local resolver found for '{provider}'");
    }

    public ISmsSender GetSender(SmsProviders provider)
    {
        if (_providers.TryGetValue(provider, out var senderProvider))
        {
            return senderProvider.ResolveSender();
        }

        throw new KeyNotFoundException($"No sender resolver found for '{provider}'");
    }
}