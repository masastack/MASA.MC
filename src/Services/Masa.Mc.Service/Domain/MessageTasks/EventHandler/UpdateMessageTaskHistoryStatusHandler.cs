// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class UpdateMessageTaskHistoryStatusHandler
{
    private readonly IMessageTaskHistoryRepository _historyRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public UpdateMessageTaskHistoryStatusHandler(IMessageTaskHistoryRepository historyRepository
        , IMessageRecordRepository messageRecordRepository)
    {
        _historyRepository = historyRepository;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(UpdateMessageTaskHistoryStatusEvent eto)
    {
        var history = await _historyRepository.FindAsync(x => x.Id == eto.MessageTaskHistoryId, false);
        if (history == null) return;

        if (history.Status == MessageTaskHistoryStatuses.Withdrawn)
        {
            return;
        }

        if (await _messageRecordRepository.AnyAsync(x => x.MessageTaskHistoryId == history.Id && x.Success == null))
        {
            return;
        }

        var hasSuccessMessageRecord = await _messageRecordRepository.AnyAsync(x => x.MessageTaskHistoryId == history.Id && x.Success == true);
        var hasFailMessageRecord = await _messageRecordRepository.AnyAsync(x => x.MessageTaskHistoryId == history.Id && x.Success == false);

        if (!hasSuccessMessageRecord && !hasFailMessageRecord)
        {
            return;
        }

        MessageTaskHistoryStatuses targetStatus;
        if (!hasFailMessageRecord)
        {
            targetStatus = MessageTaskHistoryStatuses.Success;
        }
        else if (!hasSuccessMessageRecord)
        {
            targetStatus = MessageTaskHistoryStatuses.Fail;
        }
        else
        {
            targetStatus = MessageTaskHistoryStatuses.PartialFailure;
        }

        if (history.Status == targetStatus)
        {
            return;
        }

        history.SetResult(targetStatus);

        await _historyRepository.UpdateAsync(history);
    }
}
