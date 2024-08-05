// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor<TOptions> : IOptionsResolveContributor<TOptions>
{
    public const string CONTRIBUTOR_NAME = "AsyncLocal";

    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(OptionsResolveContext<TOptions> context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IAsyncLocalAccessor<TOptions>>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}
