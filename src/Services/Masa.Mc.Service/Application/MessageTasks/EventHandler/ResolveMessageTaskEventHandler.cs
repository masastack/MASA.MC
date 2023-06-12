// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    private readonly IMessageTaskRepository _messageTaskRepository;

    public ResolveMessageTaskEventHandler(IMessageTaskRepository messageTaskRepository)
    {
        _messageTaskRepository = messageTaskRepository;
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
            OperatorId = eto.OperatorId
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
