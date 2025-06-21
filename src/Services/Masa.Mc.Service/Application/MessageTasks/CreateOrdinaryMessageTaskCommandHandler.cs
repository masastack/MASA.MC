// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class CreateOrdinaryMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IUserContext _userContext;

    public CreateOrdinaryMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageInfoRepository messageInfoRepositor, IUserContext userContext)
    {
        _domainService = domainService;
        _messageInfoRepository = messageInfoRepositor;
        _userContext = userContext;
    }

    [EventHandler(1)]
    public async Task CreateMessageInfoAsync(CreateOrdinaryMessageTaskCommand createCommand)
    {
        var messageInfo = createCommand.MessageTask.MessageInfo.Adapt<MessageInfo>();
        await _messageInfoRepository.AddAsync(messageInfo);
        await _messageInfoRepository.UnitOfWork.SaveChangesAsync();
        createCommand.MessageTask.EntityId = messageInfo.Id;
        createCommand.MessageTask.DisplayName = messageInfo.MessageContent.Title;
    }

    [EventHandler(2)]
    public async Task CreateOrdinaryMessageTaskAsync(CreateOrdinaryMessageTaskCommand createCommand)
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
