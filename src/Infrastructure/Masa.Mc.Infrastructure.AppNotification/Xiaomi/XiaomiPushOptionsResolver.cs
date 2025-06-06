// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Xiaomi;

public class XiaomiPushOptionsResolver : IOptionsResolver<IXiaomiPushOptions>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ResolveOptions<IXiaomiPushOptions> _options;

    public XiaomiPushOptionsResolver(IServiceProvider serviceProvider,
        IOptions<ResolveOptions<IXiaomiPushOptions>> resolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = resolveOptions.Value;
    }

    public async Task<IXiaomiPushOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new OptionsResolveContext<IXiaomiPushOptions>(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new XiaomiPushOptions();
    }
}
