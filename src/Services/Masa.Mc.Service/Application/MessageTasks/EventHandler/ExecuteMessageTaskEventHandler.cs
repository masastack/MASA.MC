// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ExecuteMessageTaskEventHandler
{
    private readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public ExecuteMessageTaskEventHandler(IMultiEnvironmentContext multiEnvironmentContext)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler]
    public async Task HandleEventAsync(ExecuteMessageTaskEvent eto)
    {
        var args = new ExecuteMessageTaskJobArgs()
        {
            MessageTaskId = eto.MessageTaskId,
            IsTest = eto.IsTest,
            JobId = eto.JobId,
            TaskId = eto.TaskId,
            Environment = _multiEnvironmentContext.CurrentEnvironment
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}