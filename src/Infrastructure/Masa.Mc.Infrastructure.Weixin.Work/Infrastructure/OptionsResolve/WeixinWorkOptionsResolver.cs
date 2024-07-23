// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve;

public class WeixinWorkOptionsResolver : IWeixinWorkOptionsResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly WeixinWorkResolveOptions _options;

    public WeixinWorkOptionsResolver(IServiceProvider serviceProvider,
        IOptions<WeixinWorkResolveOptions> aliyunSmsResolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = aliyunSmsResolveOptions.Value;
    }

    public async Task<IWeixinWorkOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new WeixinWorkOptionsResolveContext(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new WeixinWorkOptions();
    }
}
