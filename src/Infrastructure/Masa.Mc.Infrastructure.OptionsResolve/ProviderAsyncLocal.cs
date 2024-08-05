// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve;

public class ProviderAsyncLocal<TOptions> : IProviderAsyncLocal<TOptions>
{
    public TOptions CurrentOptions { get; private set; }

    private readonly IAsyncLocalAccessor<TOptions> _asyncLocalAccessor;

    public ProviderAsyncLocal(IAsyncLocalAccessor<TOptions> asyncLocalAccessor)
    {
        _asyncLocalAccessor = asyncLocalAccessor;

        CurrentOptions = asyncLocalAccessor.Current;
    }

    public IDisposable Change(TOptions options)
    {
        var parentScope = _asyncLocalAccessor.Current;

        _asyncLocalAccessor.Current = options;

        return new DisposeAction(() =>
        {
            _asyncLocalAccessor.Current = parentScope;
        });
    }
}
