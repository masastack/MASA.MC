// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve;

public class ResolveOptions<TOptions>
{
    public List<IOptionsResolveContributor<TOptions>> Contributors { get; }

    public ResolveOptions()
    {
        Contributors = new List<IOptionsResolveContributor<TOptions>>();
    }
}
