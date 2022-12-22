// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageInfos;

public class MessageInfoCommandHandler
{
    private readonly IMessageInfoRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly II18n<DefaultResource> _i18n;

    public MessageInfoCommandHandler(IMessageInfoRepository repository, IIntegrationEventBus integrationEventBus, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageInfoCommand createCommand)
    {
        var entity = createCommand.MessageInfo.Adapt<MessageInfo>();
        await _repository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateMessageInfoCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageInfoId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageInfo"));

        updateCommand.MessageInfo.Adapt(entity);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageInfoCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageInfoId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageInfo"));

        await _repository.RemoveAsync(entity);
    }
}
