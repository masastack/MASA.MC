// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
    public async Task UpdateOrdinaryMessageTaskAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, "MessageTask");

        var dto = updateCommand.MessageTask;
        var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entity.EntityId);
        MasaArgumentException.ThrowIfNull(messageInfo, "MessageInfo");
        dto.MessageInfo.Adapt(messageInfo);
        await _messageInfoRepository.UpdateAsync(messageInfo);
        dto.DisplayName = messageInfo.MessageContent.Title;
        dto.EntityId = messageInfo.Id;

        if (entity.Status != MessageTaskStatuses.WaitSend)
            throw new UserFriendlyException("It can only be modified after being sent");

        dto.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }
}
