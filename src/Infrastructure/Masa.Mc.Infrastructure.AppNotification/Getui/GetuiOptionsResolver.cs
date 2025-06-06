// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Getui;

public class GetuiOptionsResolver : IOptionsResolver<IGetuiOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IGetuiOptions> _options;

    public GetuiOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IGetuiOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IGetuiOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IGetuiOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new GetuiOptions();
    }
}
