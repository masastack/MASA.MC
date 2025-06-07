// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Vivo;

public class VivoPushOptionsResolver : IOptionsResolver<IVivoPushOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IVivoPushOptions> _options;

    public VivoPushOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IVivoPushOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IVivoPushOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IVivoPushOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new VivoPushOptions();
    }
}
