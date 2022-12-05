// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Entities;
using Masa.Mc.Service.Admin.Domain.MessageInfos.Aggregates;

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class UpdateOrdinaryMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;

    public UpdateOrdinaryMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTaskRepository repository, IMessageInfoRepository messageInfoRepositor)
    {
        _domainService = domainService;
        _repository = repository;
        _messageInfoRepository = messageInfoRepositor;
    }

    [EventHandler(1)]
    public async Task UpdateMessageInfoAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var dto = updateCommand.MessageTask;
        var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == dto.EntityId);
        MasaArgumentException.ThrowIfNull(messageInfo, "MessageInfo");

        dto.MessageInfo.Adapt(messageInfo);
        await _messageInfoRepository.UpdateAsync(messageInfo);
        updateCommand.MessageTask.DisplayName = messageInfo.MessageContent.Title;
    }

    [EventHandler(2)]
    public async Task UpdateOrdinaryMessageTaskAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, "MessageInfo");
        if (!entity.IsDraft)
            throw new UserFriendlyException("non draft cannot be modified");
        var dto = updateCommand.MessageTask;
        dto.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }
}
