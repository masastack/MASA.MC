// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskHistoryCommandHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly IMessageTaskJobService _messageTaskJobService;
    private readonly IUserContext _userContext;

    public MessageTaskHistoryCommandHandler(IMessageTaskHistoryRepository repository
        , IMessageTaskJobService messageTaskJobService
        , IUserContext userContext)
    {
        _repository = repository;
        _messageTaskJobService = messageTaskJobService;
        _userContext = userContext;
    }

    [EventHandler]
    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryCommand command)
    {
        var entity = (await _repository.GetQueryableAsync()).Include(x => x.MessageTask).FirstOrDefault(x => x.Id == command.MessageTaskHistoryId);
        if (entity == null)
            throw new UserFriendlyException("messageHistory not found");
        if (entity.Status == MessageTaskHistoryStatuses.Withdrawn)
            throw new UserFriendlyException("withdrawn");
        entity.SetWithdraw();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerTaskId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _messageTaskJobService.StopTaskAsync(entity.SchedulerTaskId, userId);
        }
    }
}
