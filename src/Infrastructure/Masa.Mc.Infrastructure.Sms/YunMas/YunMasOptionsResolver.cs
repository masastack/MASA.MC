// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.YunMas;

public class YunMasOptionsResolver : IOptionsResolver<IYunMasOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IYunMasOptions> _options;

    public YunMasOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IYunMasOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IYunMasOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IYunMasOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new YunMasOptions();
    }
}
