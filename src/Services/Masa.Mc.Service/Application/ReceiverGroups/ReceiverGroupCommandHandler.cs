// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupCommandHandler
{
    private readonly IReceiverGroupRepository _repository;
    private readonly ReceiverGroupDomainService _domainService;

    public ReceiverGroupCommandHandler(IReceiverGroupRepository repository, ReceiverGroupDomainService domainService)
    {
        _repository = repository;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task CreateAsync(CreateReceiverGroupCommand createCommand)
    {
        var entity = createCommand.ReceiverGroup.Adapt<ReceiverGroup>();
        var items = createCommand.ReceiverGroup.Items.Adapt<List<ReceiverGroupItem>>();
        await _domainService.CreateAsync(entity, items);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateReceiverGroupCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.ReceiverGroupId);
        MasaArgumentException.ThrowIfNull(entity, "ReceiverGroup");

        updateCommand.ReceiverGroup.Adapt(entity);
        var items = updateCommand.ReceiverGroup.Items.Adapt<List<ReceiverGroupItem>>();
        await _domainService.UpdateAsync(entity, items);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteReceiverGroupCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ReceiverGroupId);
        MasaArgumentException.ThrowIfNull(entity, "ReceiverGroup");

        await _repository.RemoveAsync(entity);
    }
}