// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly ITemplateRenderer _templateRenderer;

    public MessageTaskDomainService(IDomainEventBus eventBus
        , IMessageTaskRepository repository
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository
        , ITemplateRenderer templateRenderer) : base(eventBus)
    {
        _repository = repository;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _templateRenderer = templateRenderer;
    }

    public virtual async Task CreateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SelectReceiverType, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.AddAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, messageTask.ReceiverType, messageTask.SelectReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables));
        }
    }

    public virtual async Task UpdateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SendTask(messageTask.ReceiverType, messageTask.Receivers, messageTask.SelectReceiverType, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDraft(true);
        }
        await _repository.UpdateAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, messageTask.ReceiverType, messageTask.SelectReceiverType, messageTask.Receivers, messageTask.SendRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables));
        }
    }

    public virtual async Task SendAsync(Guid messageTaskId, ReceiverTypes receiverType, MessageTaskSelectReceiverTypes selectReceiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTimeOffset? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        if (!messageTask.IsEnabled)
            throw new UserFriendlyException("cannot send when disabled");
        messageTask.SendTask(receiverType, receivers, selectReceiverType, sendRules, sendTime, sign, variables);
        await _repository.UpdateAsync(messageTask);
        await _repository.UnitOfWork.SaveChangesAsync();
        await EventBus.PublishAsync(new AddMessageTaskHistoryEvent(messageTask, receiverType, selectReceiverType, receivers, sendRules, sendTime, sign, variables));
    }

    public virtual async Task<MessageData> GetMessageDataAsync(MessageEntityTypes entityType, Guid entityId, ExtraPropertyDictionary variables = null)
    {
        var messageData = new MessageData();
        if (entityType == MessageEntityTypes.Ordinary)
        {
            var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entityId);
            messageData = new MessageData { ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageInfo) };
        }
        if (entityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == entityId);
            messageData = new MessageData { ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageTemplate) };
        }
        if (variables != null)
        {
            messageData.SetDataValue(nameof(MessageTemplate.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), variables));
            messageData.SetDataValue(nameof(MessageTemplate.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), variables));
        }
        return messageData;
    }
}
