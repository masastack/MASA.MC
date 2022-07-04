// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class UpdateTemplateMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IMessageTaskRepository _repository;

    public UpdateTemplateMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTemplateRepository messageTemplateRepository, IMessageTaskRepository repository)
    {
        _domainService = domainService;
        _messageTemplateRepository = messageTemplateRepository;
        _repository = repository;
    }

    [EventHandler(1)]
    public async Task CheckMessageTemplateAsync(UpdateTemplateMessageTaskCommand updateCommand)
    {
        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == updateCommand.MessageTask.EntityId);
        if (messageTemplate == null && !updateCommand.MessageTask.IsDraft)
            throw new UserFriendlyException("messageTemplate not found");

        if (messageTemplate != null)
        {
            updateCommand.MessageTask.DisplayName = string.IsNullOrEmpty(messageTemplate.Title) ? messageTemplate.DisplayName : messageTemplate.Title;
        }
    }

    [EventHandler(2)]
    public async Task UpdateTemplateMessageTaskAsync(UpdateTemplateMessageTaskCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (!entity.IsDraft)
            throw new UserFriendlyException("non draft cannot be modified");
        updateCommand.MessageTask.Adapt(entity);
        entity.UpdateVariables(updateCommand.MessageTask.Variables);
        await _domainService.UpdateAsync(entity);
    }
}
