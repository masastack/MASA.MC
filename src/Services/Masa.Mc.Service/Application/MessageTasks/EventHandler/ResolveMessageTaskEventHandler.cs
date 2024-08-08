// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public ResolveMessageTaskEventHandler(IMessageTaskRepository messageTaskRepository, IMultiEnvironmentContext multiEnvironmentContext)
    {
        _messageTaskRepository = messageTaskRepository;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler]
    public async Task HandleEventAsync(ResolveMessageTaskEvent eto)
    {
        var messageTask = await _messageTaskRepository.FindAsync(x => x.Id == eto.MessageTaskId);

        if (!messageTask.SendRules.IsCustom) {
            messageTask.SetSending();
            await _messageTaskRepository.UpdateAsync(messageTask);
        }

        var args = new ResolveMessageTaskJobArgs()
        {
            MessageTaskId = eto.MessageTaskId,
            OperatorId = eto.OperatorId,
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            TraceParent = Activity.Current?.Id
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
