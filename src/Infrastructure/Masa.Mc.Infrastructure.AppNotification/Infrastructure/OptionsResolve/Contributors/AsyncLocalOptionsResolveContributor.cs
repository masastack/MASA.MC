// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.AppNotification.Infrastructure.OptionsResolve.Contributors;

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

public class AppAsyncLocalAccessor : IAppNotificationAsyncLocalAccessor
{
    public IAppNotificationOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IAppNotificationOptions> _asyncLocal;

    public AppAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IAppNotificationOptions>();
    }
}

public class AppAsyncLocal : IAppNotificationAsyncLocal
{
    public IAppNotificationOptions CurrentOptions { get; private set; }

    private readonly IAppNotificationAsyncLocalAccessor _appAsyncLocalAccessor;

    public AppAsyncLocal(IAppNotificationAsyncLocalAccessor appAsyncLocalAccessor)
    {
        _appAsyncLocalAccessor = appAsyncLocalAccessor;

        CurrentOptions = appAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IAppNotificationOptions appOptions)
    {
        var parentScope = _appAsyncLocalAccessor.Current;

        _appAsyncLocalAccessor.Current = appOptions;

        return new DisposeAction(() =>
        {
            _appAsyncLocalAccessor.Current = parentScope;
        });
    }
}
