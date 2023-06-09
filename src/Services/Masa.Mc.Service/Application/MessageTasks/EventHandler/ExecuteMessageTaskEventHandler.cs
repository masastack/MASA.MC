﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ExecuteMessageTaskEventHandler
{
    public ExecuteMessageTaskEventHandler()
    {
    }

    [EventHandler]
    public async Task HandleEventAsync(ExecuteMessageTaskEvent eto)
    {
        var args = new ExecuteMessageTaskJobArgs()
        {
            MessageTaskId = eto.MessageTaskId,
            IsTest = eto.IsTest,
            JobId = eto.JobId,
            TaskId = eto.TaskId
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}