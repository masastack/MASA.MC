// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Sms.Aliyun.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor : IAliyunSmsOptionsResolveContributor
{
    public const string ContributorName = "AsyncLocal";

    public string Name => ContributorName;

    public Task ResolveAsync(AliyunSmsOptionsResolveContext context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IAliyunSmsAsyncLocalAccessor>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}

public interface IAliyunSmsAsyncLocalAccessor
{
    IAliyunSmsOptions Current { get; set; }
}

public class AliyunSmsAsyncLocalAccessor : IAliyunSmsAsyncLocalAccessor
{
    public IAliyunSmsOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IAliyunSmsOptions> _asyncLocal;

    public AliyunSmsAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IAliyunSmsOptions>();
    }
}

public interface IAliyunSmsAsyncLocal
{
    IAliyunSmsOptions CurrentOptions { get; }

    IDisposable Change(IAliyunSmsOptions weChatMiniProgramOptions);
}

public class AliyunSmsAsyncLocal : IAliyunSmsAsyncLocal
{
    public IAliyunSmsOptions CurrentOptions { get; private set; }

    private readonly IAliyunSmsAsyncLocalAccessor _aliyunSmsAsyncLocalAccessor;

    public AliyunSmsAsyncLocal(IAliyunSmsAsyncLocalAccessor aliyunSmsAsyncLocalAccessor)
    {
        _aliyunSmsAsyncLocalAccessor = aliyunSmsAsyncLocalAccessor;

        CurrentOptions = aliyunSmsAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IAliyunSmsOptions aliyunSmsOptions)
    {
        var parentScope = _aliyunSmsAsyncLocalAccessor.Current;

        _aliyunSmsAsyncLocalAccessor.Current = aliyunSmsOptions;

        return new DisposeAction(() =>
        {
            _aliyunSmsAsyncLocalAccessor.Current = parentScope;
        });
    }
}
