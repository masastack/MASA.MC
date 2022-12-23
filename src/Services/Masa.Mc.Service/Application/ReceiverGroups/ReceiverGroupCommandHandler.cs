// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups;

public class ReceiverGroupCommandHandler
{
    private readonly IReceiverGroupRepository _repository;
    private readonly ReceiverGroupDomainService _domainService;
    private readonly II18n<DefaultResource> _i18n;

    public ReceiverGroupCommandHandler(IReceiverGroupRepository repository, ReceiverGroupDomainService domainService, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _domainService = domainService;
        _i18n = i18n;
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
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("ReceiverGroup"));

        updateCommand.ReceiverGroup.Adapt(entity);
        var items = updateCommand.ReceiverGroup.Items.Adapt<List<ReceiverGroupItem>>();
        await _domainService.UpdateAsync(entity, items);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteReceiverGroupCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ReceiverGroupId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("ReceiverGroup"));

        await _repository.RemoveAsync(entity);
    }
}