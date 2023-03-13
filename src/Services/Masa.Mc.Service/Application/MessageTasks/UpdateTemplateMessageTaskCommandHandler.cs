// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class UpdateTemplateMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IMessageTaskRepository _repository;
    private readonly II18n<DefaultResource> _i18n;

    public UpdateTemplateMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTemplateRepository messageTemplateRepository, IMessageTaskRepository repository, II18n<DefaultResource> i18n)
    {
        _domainService = domainService;
        _messageTemplateRepository = messageTemplateRepository;
        _repository = repository;
        _i18n = i18n;
    }

    [EventHandler(1)]
    public async Task CheckMessageTemplateAsync(UpdateTemplateMessageTaskCommand updateCommand)
    {
        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == updateCommand.MessageTask.EntityId);
        if (messageTemplate == null && !updateCommand.MessageTask.IsDraft)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TEMPLATE_NOT_EXIST);

        if (messageTemplate != null)
        {
            updateCommand.MessageTask.DisplayName = string.IsNullOrEmpty(messageTemplate.MessageContent.Title) ? messageTemplate.DisplayName : messageTemplate.MessageContent.Title;
        }
    }

    [EventHandler(2)]
    public async Task UpdateTemplateMessageTaskAsync(UpdateTemplateMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        if (entity.Status != MessageTaskStatuses.WaitSend)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CAN_BE_MODIFIED_BEFORE_SENDING);
        updateCommand.MessageTask.Adapt(entity);
        entity.UpdateVariables(updateCommand.MessageTask.Variables);
        await _domainService.UpdateAsync(entity);
    }
}
