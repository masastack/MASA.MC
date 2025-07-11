// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public class YunMasSmsSenderProvider : ISmsSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public SmsProviders Provider => SmsProviders.YunMas;

    public YunMasSmsSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<YunMasOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IYunMasOptions>>();
    }

    public ISmsSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<YunMasSmsSender>();
    }
}