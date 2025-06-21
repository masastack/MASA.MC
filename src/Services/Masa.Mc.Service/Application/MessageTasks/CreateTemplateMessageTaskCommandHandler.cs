// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class CreateTemplateMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IUserContext _userContext;

    public CreateTemplateMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageTemplateRepository messageTemplateRepository, IUserContext userContext)
    {
        _domainService = domainService;
        _messageTemplateRepository = messageTemplateRepository;
        _userContext = userContext;
    }

    [EventHandler(1)]
    public async Task CheckMessageTemplateAsync(CreateTemplateMessageTaskCommand createCommand)
    {
        var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == createCommand.MessageTask.EntityId);
        if (messageTemplate == null && !createCommand.MessageTask.IsDraft)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TEMPLATE_NOT_EXIST);

        if (messageTemplate != null)
        {
            createCommand.MessageTask.DisplayName = string.IsNullOrEmpty(messageTemplate.MessageContent.Title) ? messageTemplate.DisplayName : messageTemplate.MessageContent.Title;
            if (!createCommand.MessageTask.IsDraft && string.IsNullOrEmpty(createCommand.MessageTask.Sign))
            {
                createCommand.MessageTask.Sign = messageTemplate.Sign;
            }

            if (messageTemplate.IsWebsiteMessage)
            {
                createCommand.MessageTask.ExtraProperties.SetProperty(BusinessConsts.IS_WEBSITE_MESSAGE, messageTemplate.IsWebsiteMessage);
            }
        }
    }

    [EventHandler(2)]
    public async Task CreateTemplateMessageTaskAsync(CreateTemplateMessageTaskCommand createCommand)
    {
        var entity = createCommand.MessageTask.Adapt<MessageTask>();
        var systemId = createCommand.MessageTask.SystemId;

        if (string.IsNullOrEmpty(systemId))
        {
            systemId = _userContext.GetUser<McUser>()?.ClientId ?? string.Empty;
        }

        await _domainService.CreateAsync(entity, systemId);
    }
}
