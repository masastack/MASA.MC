// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public MessageTaskDomainService(IDomainEventBus eventBus
        , IMessageTaskRepository repository
        , IUnitOfWork unitOfWork
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository) : base(eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
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
            await PublishMessageAsync(messageTask);
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
            await PublishMessageAsync(messageTask);
        }
    }

    public virtual async Task SendAsync(Guid messageTaskId, ReceiverTypes receiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        messageTask.SendTask(receiverType, receivers, sendRules, sendTime, sign, variables);
        await _repository.UpdateAsync(messageTask);
        await PublishMessageAsync(messageTask);
    }

    private async Task PublishMessageAsync(MessageTask messageTask)
    {
        var messageData = await GetMessageDataAsync(messageTask.EntityType, messageTask.EntityId);
        Task.Run(async () =>
        {
            await EventBus.PublishAsync(new CreateMessageEvent(messageTask.ChannelId, messageData, messageTask.Historys.LastOrDefault()));
        });
    }

    private async Task<MessageData> GetMessageDataAsync(MessageEntityTypes entityType, Guid entityId)
    {
        if (entityType == MessageEntityTypes.Ordinary)
        {
            var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entityId);
            return new MessageData { ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageInfo) };
        }
        if (entityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == entityId);
            var messageData = new MessageData { ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageTemplate) };
            return messageData;
        }
        return new MessageData();
    }

}
