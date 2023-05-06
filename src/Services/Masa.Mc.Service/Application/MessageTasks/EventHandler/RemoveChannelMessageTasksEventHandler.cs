// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class RemoveChannelMessageTasksEventHandler
{
    private readonly IMessageTaskRepository _repository;

    public RemoveChannelMessageTasksEventHandler(IMessageTaskRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(RemoveChannelMessageTasksDomainEvent eto)
    {
        var list = await _repository.GetListAsync(x => x.ChannelId == eto.ChannelId);

        await _repository.RemoveRangeAsync(list);
    }
}