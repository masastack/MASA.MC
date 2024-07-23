// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Weixin.Work.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor : IWeixinWorkOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "AsyncLocal";

    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(WeixinWorkOptionsResolveContext context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IWeixinWorkAsyncLocalAccessor>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}

public interface IWeixinWorkAsyncLocalAccessor
{
    IWeixinWorkOptions Current { get; set; }
}

public class WeixinWorkAsyncLocalAccessor : IWeixinWorkAsyncLocalAccessor
{
    public IWeixinWorkOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IWeixinWorkOptions> _asyncLocal;

    public WeixinWorkAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IWeixinWorkOptions>();
    }
}

public interface IWeixinWorkAsyncLocal
{
    IWeixinWorkOptions CurrentOptions { get; }

    IDisposable Change(IWeixinWorkOptions weChatMiniProgramOptions);
}

public class WeixinWorkAsyncLocal : IWeixinWorkAsyncLocal
{
    public IWeixinWorkOptions CurrentOptions { get; private set; }

    private readonly IWeixinWorkAsyncLocalAccessor _weixinWorkAsyncLocalAccessor;

    public WeixinWorkAsyncLocal(IWeixinWorkAsyncLocalAccessor weixinWorkAsyncLocalAccessor)
    {
        _weixinWorkAsyncLocalAccessor = weixinWorkAsyncLocalAccessor;

        CurrentOptions = weixinWorkAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IWeixinWorkOptions weixinWorkOptions)
    {
        var parentScope = _weixinWorkAsyncLocalAccessor.Current;

        _weixinWorkAsyncLocalAccessor.Current = weixinWorkOptions;

        return new DisposeAction(() =>
        {
            _weixinWorkAsyncLocalAccessor.Current = parentScope;
        });
    }
}
