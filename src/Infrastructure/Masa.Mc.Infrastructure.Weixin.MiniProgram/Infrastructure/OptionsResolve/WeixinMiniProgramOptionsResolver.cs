// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.MiniProgram.Infrastructure.OptionsResolve;

public class WeixinMiniProgramOptionsResolver : IOptionsResolver<IWeixinMiniProgramOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IWeixinMiniProgramOptions> _options;

    public WeixinMiniProgramOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IWeixinMiniProgramOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IWeixinMiniProgramOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IWeixinMiniProgramOptions>(serviceScope.ServiceProvider);

            foreach (var contributor in _options.Contributors)
            {
                await contributor.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new WeixinMiniProgramOptions();
    }
}
