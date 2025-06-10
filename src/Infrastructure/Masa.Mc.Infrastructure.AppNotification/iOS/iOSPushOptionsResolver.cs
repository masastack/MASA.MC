// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.iOS;

public class iOSPushOptionsResolver : IOptionsResolver<IiOSPushOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IiOSPushOptions> _options;

    public iOSPushOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IiOSPushOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IiOSPushOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IiOSPushOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new iOSPushOptions();
    }
}
