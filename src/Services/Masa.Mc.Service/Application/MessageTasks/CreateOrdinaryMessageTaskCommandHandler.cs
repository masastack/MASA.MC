﻿namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class CreateOrdinaryMessageTaskCommandHandler
{
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageInfoRepository _messageInfoRepository;

    public CreateOrdinaryMessageTaskCommandHandler(MessageTaskDomainService domainService, IMessageInfoRepository messageInfoRepositor)
    {
        _domainService = domainService;
        _messageInfoRepository = messageInfoRepositor;
    }

    [EventHandler(1)]
    public async Task CreateMessageInfoAsync(CreateOrdinaryMessageTaskCommand createCommand)
    {
        var messageInfo = createCommand.MessageTask.MessageInfo.Adapt<MessageInfo>();
        await _messageInfoRepository.AddAsync(messageInfo);
        createCommand.MessageTask.EntityId = messageInfo.Id;
        createCommand.MessageTask.DisplayName = messageInfo.Title;
    }

    [EventHandler(2)]
    public async Task CreateOrdinaryMessageTaskAsync(CreateOrdinaryMessageTaskCommand createCommand)
    {
        var entity = createCommand.MessageTask.Adapt<MessageTask>();
        await _domainService.CreateAsync(entity);
    }
}