// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskHistoryCommandHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly IMessageTaskJobService _messageTaskJobService;
    private readonly IUserContext _userContext;
    private readonly II18n<DefaultResource> _i18n;

    public MessageTaskHistoryCommandHandler(IMessageTaskHistoryRepository repository
        , IMessageTaskJobService messageTaskJobService
        , IUserContext userContext
        , II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _messageTaskJobService = messageTaskJobService;
        _userContext = userContext;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryCommand command)
    {
        var entity = (await _repository.GetQueryableAsync()).Include(x => x.MessageTask).FirstOrDefault(x => x.Id == command.MessageTaskHistoryId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTaskHistory"));
        if (entity.Status == MessageTaskHistoryStatuses.Withdrawn)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TASK_HISTORY_WITHDRAWN);
        entity.SetWithdraw();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerTaskId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _messageTaskJobService.StopTaskAsync(entity.SchedulerTaskId, userId);
        }
    }
}
