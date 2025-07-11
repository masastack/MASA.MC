﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun;

public class AliyunSmsOptionsResolver : IOptionsResolver<IAliyunSmsOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IAliyunSmsOptions> _options;

    public AliyunSmsOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IAliyunSmsOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IAliyunSmsOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IAliyunSmsOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new AliyunSmsOptions();
    }
}
