// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class AddMessageTaskHistoryEventHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly IDomainEventBus _eventBus;
    private readonly MessageTaskDomainService _messageTaskDomainService;
    public AddMessageTaskHistoryEventHandler(IMessageTaskHistoryRepository repository
        , IDomainEventBus eventBus
        , MessageTaskDomainService messageTaskDomainService)
    {
        _repository = repository;
        _eventBus = eventBus;
        _messageTaskDomainService = messageTaskDomainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(AddMessageTaskHistoryEvent eto)
    {
        var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
        var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, eto.ReceiverType, eto.selectReceiverType, eto.Receivers, eto.SendRules, eto.SendTime, eto.Sign, eto.Variables);
        await _repository.AddAsync(history);
        await _repository.UnitOfWork.SaveChangesAsync();
        await _repository.UnitOfWork.CommitAsync();
        await PublishMessageAsync(eto.MessageTask, history);
    }

    private async Task PublishMessageAsync(MessageTask messageTask, MessageTaskHistory messageTaskHistory)
    {
        var messageData = await _messageTaskDomainService.GetMessageDataAsync(messageTask.EntityType, messageTask.EntityId);
        messageData.SetDataValue(nameof(MessageTemplate.Sign), messageTaskHistory.Sign);
        await _eventBus.PublishAsync(new CreateMessageEvent(messageTask.ChannelId, messageData, messageTaskHistory.Id));
    }
}
