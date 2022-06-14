// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskHistoryDomainService : DomainService
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly ITemplateRenderer _templateRenderer;

    public MessageTaskHistoryDomainService(IDomainEventBus eventBus
        , IMessageTaskHistoryRepository repository
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository
        , ITemplateRenderer templateRenderer) : base(eventBus)
    {
        _repository = repository;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _templateRenderer = templateRenderer;
    }

    public virtual async Task ExecuteAsync(Guid messageTaskHistoryId)
    {
        var messageTaskHistory = await _repository.FindAsync(x=>x.Id== messageTaskHistoryId);
        var messageData = await GetMessageDataAsync(messageTaskHistory.MessageTask.EntityType, messageTaskHistory.MessageTask.EntityId);
        messageData.SetDataValue(nameof(MessageTemplate.Sign), messageTaskHistory.Sign);
        await EventBus.PublishAsync(new CreateMessageEvent(messageTaskHistory.MessageTask.ChannelId, messageData, messageTaskHistory.Id));
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
