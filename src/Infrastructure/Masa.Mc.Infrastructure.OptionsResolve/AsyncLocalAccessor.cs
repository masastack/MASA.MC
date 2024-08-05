// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.OptionsResolve;

public class AsyncLocalAccessor<TOptions> : IAsyncLocalAccessor<TOptions>
{
    public TOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<TOptions> _asyncLocal;

    public AsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<TOptions>();
    }
}
