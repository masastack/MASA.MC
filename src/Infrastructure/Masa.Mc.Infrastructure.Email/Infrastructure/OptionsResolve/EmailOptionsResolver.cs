// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve;

public class EmailOptionsResolver : IEmailOptionsResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EmailResolveOptions _options;

    public EmailOptionsResolver(IServiceProvider serviceProvider,
        IOptions<EmailResolveOptions> aliyunSmsResolveOptions)
    {
        _serviceProvider = serviceProvider;
        _options = aliyunSmsResolveOptions.Value;
    }

    public async Task<IEmailOptions> ResolveAsync()
    {
        using (var serviceScope = _serviceProvider.CreateScope())
        {
            var context = new EmailOptionsResolveContext(serviceScope.ServiceProvider);

            foreach (var resolver in _options.Contributors)
            {
                await resolver.ResolveAsync(context);

                if (context.Options != null)
                {
                    return context.Options;
                }
            }
        }

        return new SmtpEmailOptions();
    }
}
