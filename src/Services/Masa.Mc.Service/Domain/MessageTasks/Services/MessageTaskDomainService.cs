﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageTaskDomainService(IDomainEventBus eventBus, IMessageTaskRepository repository, IUnitOfWork unitOfWork) : base(eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task CreateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendingRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.AddAsync(messageTask);
    }

    public virtual async Task UpdateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendingRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.UpdateAsync(messageTask);
    }

    public virtual async Task SendAsync(Guid messageTaskId, ReceiverType receiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendingRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        messageTask.SendTask(receiverType, receivers, sendingRules, sendTime, sign, variables);
        await _repository.UpdateAsync(messageTask);
    }
}
