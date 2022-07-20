// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskHistoryCommandHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly ISchedulerClient _schedulerClient;
    private readonly IUserContext _userContext;

    public MessageTaskHistoryCommandHandler(IMessageTaskHistoryRepository repository
        , ISchedulerClient schedulerClient
        , IUserContext userContext)
    {
        _repository = repository;
        _schedulerClient = schedulerClient;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryCommand command)
    {
        var entity = (await _repository.GetQueryableAsync()).Include(x => x.MessageTask).FirstOrDefault(x => x.Id == command.Input.HistoryId);
        if (entity == null)
            throw new UserFriendlyException("messageHistory not found");
        if (entity.Status == MessageTaskHistoryStatuses.Withdrawn)
            throw new UserFriendlyException("withdrawn");
        entity.SetWithdraw();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerTaskId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _schedulerClient.SchedulerTaskService.StopAsync(new BaseSchedulerTaskRequest { TaskId = entity.SchedulerTaskId, OperatorId = userId });
        }
    }
}
