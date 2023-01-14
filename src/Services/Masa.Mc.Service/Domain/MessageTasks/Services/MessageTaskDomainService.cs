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
    private readonly II18n<DefaultResource> _i18n;

    public MessageTaskDomainService(IDomainEventBus eventBus
        , IMessageTaskRepository repository
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository
        , ITemplateRenderer templateRenderer
        , IChannelRepository channelRepository
        , II18n<DefaultResource> i18n) : base(eventBus)
    {
        _repository = repository;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _templateRenderer = templateRenderer;
        _channelRepository = channelRepository;
        _i18n = i18n;
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

    public virtual async Task<MessageData?> GetMessageDataAsync(MessageEntityTypes entityType, Guid entityId, ExtraPropertyDictionary variables)
    {
        if (entityType == MessageEntityTypes.Ordinary)
        {
            var messageInfo = await _messageInfoRepository.FindAsync(x => x.Id == entityId);
            if (messageInfo == null) return null;

            var messageData = new MessageData(messageInfo.MessageContent, MessageEntityTypes.Ordinary);
            messageData.RenderContent(variables);
            return messageData;
        }
        if (entityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _messageTemplateRepository.FindAsync(x => x.Id == entityId);
            if (messageTemplate == null) return null;

            var messageData = new MessageData(messageTemplate.MessageContent, MessageEntityTypes.Template);
            messageData.RenderContent(variables);
            messageData.SetDataValue(nameof(MessageTemplate.TemplateId), messageTemplate.TemplateId);
            messageData.SetDataValue(nameof(MessageTemplate.Sign), messageTemplate.Sign);
            return messageData;
        }

        return null;
    }

    public virtual async Task<MessageData?> GetMessageDataAsync(Guid messageTaskId, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        MasaArgumentException.ThrowIfNull(messageTask, _i18n.T("MessageTask"));

        return await GetMessageDataAsync(messageTask.EntityType, messageTask.EntityId, variables);
    }

    protected async Task ValidateChannelAsync(MessageTask messageTask)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == messageTask.ChannelId);
        MasaArgumentException.ThrowIfNull(channel, _i18n.T("Channel"));

        if (channel.Type.Id != messageTask.ChannelType?.Id)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_TYPE_DOES_NOT_MATCH_CHANNEL);
    }
}
