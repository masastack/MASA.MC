// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IChannelRepository _channelRepository;

    public MessageTaskDomainService(IDomainEventBus eventBus
        , IMessageTaskRepository repository
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository
        , ITemplateRenderer templateRenderer
        , IChannelRepository channelRepository) : base(eventBus)
    {
        _repository = repository;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _templateRenderer = templateRenderer;
        _channelRepository = channelRepository;
    }

    public virtual async Task CreateAsync(MessageTask messageTask, Guid operatorId = default)
    {
        if (!messageTask.IsDraft)
        {
            await ValidateChannelAsync(messageTask);
        }

        messageTask.SetDraft(messageTask.IsDraft);
        messageTask.SetExpectSendTime();
        await _repository.AddAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new ResolveMessageTaskEvent(messageTask.Id));
        }
    }

    public virtual async Task UpdateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            await ValidateChannelAsync(messageTask);
        }

        messageTask.SetDraft(messageTask.IsDraft);
        messageTask.SetExpectSendTime();
        await _repository.UpdateAsync(messageTask);
        if (!messageTask.IsDraft)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new ResolveMessageTaskEvent(messageTask.Id));
        }
    }

    public virtual async Task<MessageData> GetMessageDataAsync(MessageEntityTypes entityType, Guid entityId, ExtraPropertyDictionary variables = null)
    {
        var messageData = new MessageData();
        if (entityType == MessageEntityTypes.Ordinary)
        {
            var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entityId);
            messageData = new MessageData { MessageType = MessageEntityTypes.Ordinary, ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageInfo) };
        }
        if (entityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == entityId);
            messageData = new MessageData { MessageType = MessageEntityTypes.Template, ExtraProperties = ExtensionPropertyHelper.ObjMapToExtraProperty(messageTemplate) };
            messageData.TemplateItems = messageTemplate.Items.ToList();
        }
        if (variables != null)
        {
            messageData.SetDataValue(nameof(MessageContent.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageContent.Title)), variables));
            messageData.SetDataValue(nameof(MessageContent.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageContent.Content)), variables));
            messageData.SetDataValue(nameof(MessageContent.JumpUrl), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageContent.JumpUrl)), variables));
        }
        return messageData;
    }

    public virtual async Task<MessageData> GetMessageDataAsync(Guid messageTaskId, ExtraPropertyDictionary variables = null)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("MessageTask not found");
        return await GetMessageDataAsync(messageTask.EntityType, messageTask.EntityId, variables);
    }

    protected async Task ValidateChannelAsync(MessageTask messageTask)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == messageTask.ChannelId);
        if (channel == null)
            throw new UserFriendlyException("Channel not found");
        if (channel.Type != messageTask.ChannelType)
            throw new UserFriendlyException("Channel type does not match the channel");
    }
}
