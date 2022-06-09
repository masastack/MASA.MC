﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskHistoryCommandHandler
{
    private readonly IMessageTaskHistoryRepository _repository;

    public MessageTaskHistoryCommandHandler(IMessageTaskHistoryRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.HistoryId);
        if (entity == null)
            throw new UserFriendlyException("messageHistory not found");
        if (entity.Status == MessageTaskHistoryStatuses.Withdrawn)
            throw new UserFriendlyException("withdrawn");
        entity.SetWithdraw();
        await _repository.UpdateAsync(entity);
    }
}
