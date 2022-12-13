// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Getui.Infrastructure.OptionsResolve;

public class GetuiOptionsResolver : IAppNotificationOptionsResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly GetuiResolveOptions _options;

    public GetuiOptionsResolver(IServiceProvider serviceProvider,
        IOptions<GetuiResolveOptions> aliyunSmsResolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = aliyunSmsResolveOptions.Value;
    }

    public async Task<IAppNotificationOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new AppNotificationOptionsResolveContext(serviceScope.ServiceProvider);

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
