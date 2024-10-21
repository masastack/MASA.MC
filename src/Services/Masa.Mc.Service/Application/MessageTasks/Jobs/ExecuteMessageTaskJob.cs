// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ExecuteMessageTaskJob : BackgroundJobBase<ExecuteMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteMessageTaskJob(ILogger<BackgroundJobBase<ExecuteMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ExecuteMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(args.Environment);
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var activity = string.IsNullOrEmpty(args.TraceParent) ? default : MessageTaskExecuteJobConsts.ActivitySource.StartActivity("", ActivityKind.Consumer, args.TraceParent);

        try
        {
            var query = new ExecuteMessageTaskEvent(args.MessageTaskId, args.IsTest, args.JobId, args.TaskId);
            await eventBus.PublishAsync(query);
        }
        finally
        {
            activity?.Dispose();
        }
    }
}
