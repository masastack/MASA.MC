// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class AddMessageTaskHistoryEventHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly IMessageInfoRepository _messageInfoRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IServiceProvider _serviceProvider;
    public AddMessageTaskHistoryEventHandler(IMessageTaskHistoryRepository repository
        , IMessageInfoRepository messageInfoRepository
        , IMessageTemplateRepository messageTemplateRepository
        , IServiceProvider serviceProvider)
    {
        _repository = repository;
        _messageInfoRepository = messageInfoRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _serviceProvider = serviceProvider;
    }

    [EventHandler]
    public async Task HandleEventAsync(AddMessageTaskHistoryEvent eto)
    {
        var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
        var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, eto.ReceiverType, eto.Receivers, eto.SendRules, eto.SendTime, eto.Sign, eto.Variables);
        await _repository.AddAsync(history);
        await _repository.UnitOfWork.SaveChangesAsync();
        await _repository.UnitOfWork.CommitAsync();
        await PublishMessageAsync(eto.MessageTask, history);
    }

    private async Task PublishMessageAsync(MessageTask messageTask, MessageTaskHistory messageTaskHistory)
    {
        var messageData = await GetMessageDataAsync(messageTask.EntityType, messageTask.EntityId);
        messageData.SetDataValue(nameof(MessageTemplate.Sign), messageTaskHistory.Sign);
        Task.Run(async () =>
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var eventBus = scope.ServiceProvider.GetRequiredService<IDomainEventBus>();
                await eventBus.PublishAsync(new CreateMessageEvent(messageTask.ChannelId, messageData, messageTaskHistory.Id));
            }
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
