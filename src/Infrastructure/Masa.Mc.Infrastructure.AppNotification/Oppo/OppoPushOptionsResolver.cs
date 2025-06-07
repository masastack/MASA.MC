// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Oppo;

public class OppoPushOptionsResolver : IOptionsResolver<IOppoPushOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IOppoPushOptions> _options;

    public OppoPushOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IOppoPushOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IOppoPushOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IOppoPushOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new OppoPushOptions();
    }
}
