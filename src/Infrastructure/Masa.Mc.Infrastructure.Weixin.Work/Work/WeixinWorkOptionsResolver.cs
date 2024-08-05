// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Work;

public class WeixinWorkOptionsResolver : IOptionsResolver<IWeixinWorkOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IWeixinWorkOptions> _options;

    public WeixinWorkOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IWeixinWorkOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IWeixinWorkOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IWeixinWorkOptions>(serviceScope.ServiceProvider);

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
