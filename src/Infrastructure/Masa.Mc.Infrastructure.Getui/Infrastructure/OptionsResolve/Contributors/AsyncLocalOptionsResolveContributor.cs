// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Getui.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor : IAppNotificationOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "AsyncLocal";

    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(AppNotificationOptionsResolveContext context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IAppNotificationAsyncLocalAccessor>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}

public class GetuiAsyncLocalAccessor : IAppNotificationAsyncLocalAccessor
{
    public IAppNotificationOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IAppNotificationOptions> _asyncLocal;

    public GetuiAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IAppNotificationOptions>();
    }
}

public class GetuiAsyncLocal : IAppNotificationAsyncLocal
{
    public IAppNotificationOptions CurrentOptions { get; private set; }

    private readonly IAppNotificationAsyncLocalAccessor _getuiAsyncLocalAccessor;

    public GetuiAsyncLocal(IAppNotificationAsyncLocalAccessor getuiAsyncLocalAccessor)
    {
        _getuiAsyncLocalAccessor = getuiAsyncLocalAccessor;

        CurrentOptions = getuiAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IAppNotificationOptions getuiOptions)
    {
        var parentScope = _getuiAsyncLocalAccessor.Current;

        _getuiAsyncLocalAccessor.Current = getuiOptions;

        return new DisposeAction(() =>
        {
            _getuiAsyncLocalAccessor.Current = parentScope;
        });
    }
}
