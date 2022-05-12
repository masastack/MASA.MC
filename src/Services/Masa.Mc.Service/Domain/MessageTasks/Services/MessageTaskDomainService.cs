// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;

    public MessageTaskDomainService(IDomainEventBus eventBus
        , IMessageTaskRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public virtual async Task CreateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.AddAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, messageTask.ReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables));
        }
    }

    public virtual async Task UpdateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.UpdateAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, messageTask.ReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables));
        }
    }

    public virtual async Task SendAsync(Guid messageTaskId, ReceiverTypes receiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        messageTask.SendTask(receiverType, receivers, sendRules, sendTime, sign, variables);
        await _repository.UpdateAsync(messageTask);
        await _repository.UnitOfWork.SaveChangesAsync();
        await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, receiverType, receivers, sendRules, sendTime, sign, variables));
    }
}
