// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Email.Infrastructure.OptionsResolve.Contributors;

public class AsyncLocalOptionsResolveContributor : IEmailOptionsResolveContributor
{
    public const string CONTRIBUTOR_NAME = "AsyncLocal";

    public string Name => CONTRIBUTOR_NAME;

    public Task ResolveAsync(EmailOptionsResolveContext context)
    {
        var asyncLocal = context.ServiceProvider.GetRequiredService<IEmailAsyncLocalAccessor>();

        if (asyncLocal.Current != null)
        {
            context.Options = asyncLocal.Current;
        }

        return Task.CompletedTask;
    }
}

public interface IEmailAsyncLocalAccessor
{
    IEmailOptions Current { get; set; }
}

public class EmailAsyncLocalAccessor : IEmailAsyncLocalAccessor
{
    public IEmailOptions Current
    {
        get => _asyncLocal.Value;
        set => _asyncLocal.Value = value;
    }

    private readonly AsyncLocal<IEmailOptions> _asyncLocal;

    public EmailAsyncLocalAccessor()
    {
        _asyncLocal = new AsyncLocal<IEmailOptions>();
    }
}

public interface IEmailAsyncLocal
{
    IEmailOptions CurrentOptions { get; }

    IDisposable Change(IEmailOptions weChatMiniProgramOptions);
}

public class EmailAsyncLocal : IEmailAsyncLocal
{
    public IEmailOptions CurrentOptions { get; private set; }

    private readonly IEmailAsyncLocalAccessor _emailAsyncLocalAccessor;

    public EmailAsyncLocal(IEmailAsyncLocalAccessor emailAsyncLocalAccessor)
    {
        _emailAsyncLocalAccessor = emailAsyncLocalAccessor;

        CurrentOptions = emailAsyncLocalAccessor.Current;
    }

    public IDisposable Change(IEmailOptions emailOptions)
    {
        var parentScope = _emailAsyncLocalAccessor.Current;

        _emailAsyncLocalAccessor.Current = emailOptions;

        return new DisposeAction(() =>
        {
            _emailAsyncLocalAccessor.Current = parentScope;
        });
    }
}
