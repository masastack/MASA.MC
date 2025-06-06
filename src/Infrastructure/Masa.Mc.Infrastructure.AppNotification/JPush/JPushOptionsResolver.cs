// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.JPush;

public class JPushOptionsResolver : IOptionsResolver<IJPushOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IJPushOptions> _options;

    public JPushOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IJPushOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IJPushOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IJPushOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new JPushOptions();
    }
}
