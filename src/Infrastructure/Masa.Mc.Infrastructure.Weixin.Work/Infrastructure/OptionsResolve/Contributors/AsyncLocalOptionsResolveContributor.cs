// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor : IWeixinWorkMessageOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "AsyncLocal";

    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(WeixinWorkMessageOptionsResolveContext context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IWeixinWorkMessageAsyncLocalAccessor>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}

public interface IWeixinWorkMessageAsyncLocalAccessor
{
    IWeixinWorkMessageOptions Current { get; set; }
}

public class WeixinWorkMessageAsyncLocalAccessor : IWeixinWorkMessageAsyncLocalAccessor
{
    public IWeixinWorkMessageOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IWeixinWorkMessageOptions> _asyncLocal;

    public WeixinWorkMessageAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IWeixinWorkMessageOptions>();
    }
}

public interface IWeixinWorkMessageAsyncLocal
{
    IWeixinWorkMessageOptions CurrentOptions { get; }

    IDisposable Change(IWeixinWorkMessageOptions weChatMiniProgramOptions);
}

public class WeixinWorkMessageAsyncLocal : IWeixinWorkMessageAsyncLocal
{
    public IWeixinWorkMessageOptions CurrentOptions { get; private set; }

    private readonly IWeixinWorkMessageAsyncLocalAccessor _weixinWorkMessageAsyncLocalAccessor;

    public WeixinWorkMessageAsyncLocal(IWeixinWorkMessageAsyncLocalAccessor weixinWorkMessageAsyncLocalAccessor)
    {
        _weixinWorkMessageAsyncLocalAccessor = weixinWorkMessageAsyncLocalAccessor;

        CurrentOptions = weixinWorkMessageAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IWeixinWorkMessageOptions weixinWorkMessageOptions)
    {
        var parentScope = _weixinWorkMessageAsyncLocalAccessor.Current;

        _weixinWorkMessageAsyncLocalAccessor.Current = weixinWorkMessageOptions;

        return new DisposeAction(() =>
        {
            _weixinWorkMessageAsyncLocalAccessor.Current = parentScope;
        });
    }
}
