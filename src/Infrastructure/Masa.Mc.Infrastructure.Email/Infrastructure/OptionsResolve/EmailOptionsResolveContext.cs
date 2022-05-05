// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve;

public class EmailOptionsResolveContext
{
    public IEmailOptions Options { get; set; }

    public IServiceProvider ServiceProvider { get; }

    public EmailOptionsResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
