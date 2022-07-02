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

        var totalCount = await _historyRepository.GetCountAsync(x => x.MessageTaskId == eto.MessageTaskId);

        if (messageTask.SendRules.IsSendingInterval)
        {
            var sendNum = (long)Math.Ceiling(messageTask.ReceiverUsers.Count * 1M / messageTask.SendRules.SendingCount);
            if (totalCount < sendNum)
            {
                return;
            }
        }

        var okCount = await _historyRepository.GetCountAsync(x => x.MessageTaskId == eto.MessageTaskId && x.Status == MessageTaskHistoryStatuses.Success);
        var errorCount = await _historyRepository.GetCountAsync(x => x.MessageTaskId == eto.MessageTaskId && (x.Status == MessageTaskHistoryStatuses.Fail || x.Status == MessageTaskHistoryStatuses.PartialFailure));
        if (totalCount == okCount)
        {
            messageTask.SetResult(MessageTaskStatuses.Success);
        }
        else if (okCount > 0)
        {
            messageTask.SetResult(MessageTaskStatuses.PartialFailure);
        }
        else
        {
            messageTask.SetResult(MessageTaskStatuses.Fail);
        }
        await _repository.UpdateAsync(messageTask);
        await _repository.UnitOfWork.SaveChangesAsync();
        await _repository.UnitOfWork.CommitAsync();
    }
}
