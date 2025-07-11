// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsSenderProvider : ISmsSenderProvider
{
    private readonly IServiceProvider _serviceProvider;

    public SmsProviders Provider => SmsProviders.Aliyun;

    public AliyunSmsSenderProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IOptions ResolveOptions(ConcurrentDictionary<string, object> extraProperties)
    {
        return PushOptionsMapper.Map<AliyunSmsOptions>(extraProperties);
    }

    public IProviderAsyncLocalBase ResolveProviderAsyncLocal()
    {
        return _serviceProvider.GetRequiredService<IProviderAsyncLocal<IAliyunSmsOptions>>();
    }

    public ISmsSender ResolveSender()
    {
        return _serviceProvider.GetRequiredService<AliyunSmsSender>();
    }
}