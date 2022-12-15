// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class UpdateMessageTaskStatusEventHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _historyRepository;

    public UpdateMessageTaskStatusEventHandler(IMessageTaskHistoryRepository historyRepository
        , IMessageTaskRepository repository)
    {
        _historyRepository = historyRepository;
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(UpdateMessageTaskStatusEvent eto)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == eto.MessageTaskId, false);
        if (messageTask == null)
        {
            return;
        }

        if (await _historyRepository.AnyAsync(x => x.MessageTaskId == eto.MessageTaskId && !x.IsTest && x.Status == MessageTaskHistoryStatuses.WaitSend))
        {
            return;
        }

        var totalCount = await _historyRepository.GetCountAsync(x => x.MessageTaskId == eto.MessageTaskId && !x.IsTest);

        var okCount = await _historyRepository.GetCountAsync(x => x.MessageTaskId == eto.MessageTaskId && !x.IsTest && x.Status == MessageTaskHistoryStatuses.Success);

        if (totalCount == okCount)
        {
            messageTask.SetResult(MessageTaskStatuses.Success);
        }
        else if (!await _historyRepository.AnyAsync(x => x.MessageTaskId == eto.MessageTaskId && !x.IsTest && x.Status != MessageTaskHistoryStatuses.Withdrawn))
        {
            messageTask.SetResult(MessageTaskStatuses.Cancel);
        }
        else if (await _historyRepository.AnyAsync(x => x.MessageTaskId == eto.MessageTaskId && !x.IsTest && (x.Status == MessageTaskHistoryStatuses.PartialFailure || x.Status == MessageTaskHistoryStatuses.Success)))
        {
            messageTask.SetResult(MessageTaskStatuses.PartialFailure);
        }
        else
        {
            messageTask.SetResult(MessageTaskStatuses.Fail);
        }
        await _repository.UpdateAsync(messageTask);
    }
}
