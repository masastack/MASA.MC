// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.EventHandler;

public class ChannelTemplatesEventHandler
{
    private readonly IMessageTemplateRepository _repository;

    public ChannelTemplatesEventHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(RemoveChannelTemplatesDomainEvent eto)
    {
        var list = await _repository.GetListAsync(x => x.ChannelId == eto.ChannelId);

        await _repository.RemoveRangeAsync(list);
    }
}
