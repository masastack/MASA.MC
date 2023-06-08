﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    public ResolveMessageTaskEventHandler()
    {
    }

    [EventHandler]
    public async Task HandleEventAsync(ResolveMessageTaskEvent eto)
    {
        var args = new ResolveMessageTaskJobArgs()
        {
            MessageTaskId = eto.MessageTaskId,
            OperatorId = eto.OperatorId
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
