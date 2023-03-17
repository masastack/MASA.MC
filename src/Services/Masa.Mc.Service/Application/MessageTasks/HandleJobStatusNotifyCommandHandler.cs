// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class HandleJobStatusNotifyCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _taskHistoryRepository;

    public HandleJobStatusNotifyCommandHandler(IMessageTaskRepository repository, IMessageTaskHistoryRepository taskHistoryRepository)
    {
        _repository = repository;
        _taskHistoryRepository = taskHistoryRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(HandleJobStatusNotifyCommand command)
    {
        if (command.status != JobNotifyStatus.Delete || command.status != JobNotifyStatus.Failure) return;

        var messageTask = await _repository.FindAsync(x => x.SchedulerJobId == command.JobId, false);
        if (messageTask == null) return;

        var taskHistorys = await _taskHistoryRepository.GetListAsync(x => x.MessageTaskId == messageTask.Id && x.Status == MessageTaskHistoryStatuses.WaitSend);

        foreach (var item in taskHistorys)
        {
            item.SetResult(MessageTaskHistoryStatuses.Fail);
        }

        await _taskHistoryRepository.UpdateRangeAsync(taskHistorys);
    }
}
