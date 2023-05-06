// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class TemplateMessageTasksEventHandler
{
    private readonly IMessageTaskRepository _repository;

    public TemplateMessageTasksEventHandler(IMessageTaskRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(RemoveTemplateMessageTasksDomainEvent eto)
    {
        var list = await _repository.GetListAsync(x => x.EntityType == MessageEntityTypes.Template && x.EntityId == eto.MessageTemplateId);

        await _repository.RemoveRangeAsync(list);
    }
}