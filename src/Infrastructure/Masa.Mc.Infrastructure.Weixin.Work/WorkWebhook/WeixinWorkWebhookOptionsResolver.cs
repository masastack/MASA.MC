// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Work;

public class WeixinWorkWebhookOptionsResolver : IOptionsResolver<IWeixinWorkWebhookOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IWeixinWorkWebhookOptions> _options;

    public WeixinWorkWebhookOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IWeixinWorkWebhookOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IWeixinWorkWebhookOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IWeixinWorkWebhookOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new WeixinWorkWebhookOptions();
    }
}
