// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve;

public class OptionsResolveContext<TOptions>
{
    public TOptions Options { get; set; }

    public IServiceProvider ServiceProvider { get; }

    public OptionsResolveContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}
