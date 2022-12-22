// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class UpdateOrdinaryMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly II18n<DefaultResource> _i18n;

    public UpdateOrdinaryMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTaskRepository repository, IMessageInfoRepository messageInfoRepositor, II18n<DefaultResource> i18n)
    {
        _domainService = domainService;
        _repository = repository;
        _messageInfoRepository = messageInfoRepositor;
        _i18n = i18n;
    }

    [EventHandler(1)]
    public async Task UpdateOrdinaryMessageTaskAsync(UpdateOrdinaryMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        var dto = updateCommand.MessageTask;
        var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entity.EntityId);
        MasaArgumentException.ThrowIfNull(messageInfo, _i18n.T("MessageInfo"));
        dto.MessageInfo.Adapt(messageInfo);
        await _messageInfoRepository.UpdateAsync(messageInfo);
        dto.DisplayName = messageInfo.MessageContent.Title;
        dto.EntityId = messageInfo.Id;

        if (entity.Status != MessageTaskStatuses.WaitSend)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CAN_BE_MODIFIED_AFTER_SENDING);

        dto.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }
}
