// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WebsiteMessageCreatedEventHandler
{
    private readonly IWebsiteMessageRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _messageTaskDomainService;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public WebsiteMessageCreatedEventHandler(IWebsiteMessageRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService messageTaskDomainService
        , IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTaskDomainService = messageTaskDomainService;
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler]
    public async Task HandleEvent(WebsiteMessageCreatedDomainEvent @event)
    {
        var checkStatus = new List<MessageTaskHistoryStatuses> { MessageTaskHistoryStatuses.Sending, MessageTaskHistoryStatuses.Completed };
        var taskHistorys = await _messageTaskHistoryRepository.GetListAsync(x => x.SendTime >= @event.CheckTime && x.ReceiverType == ReceiverTypes.Broadcast && checkStatus.Contains(x.Status));
        foreach (var taskHistory in taskHistorys)
        {
            var taskHistoryDetail = await _messageTaskHistoryRepository.FindAsync(x => x.Id == taskHistory.Id, true);
            if (taskHistoryDetail == null) continue;
            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistoryDetail.MessageTask.EntityType, taskHistoryDetail.MessageTask.EntityId, taskHistoryDetail.Variables);
            var websiteMessage = new WebsiteMessage(taskHistoryDetail.MessageTask.ChannelId, @event.UserId, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), taskHistoryDetail.SendTime.Value);
            await _repository.AddAsync(websiteMessage);
            var messageRecord = new MessageRecord(@event.UserId, websiteMessage.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, taskHistory.Variables);
            await _messageRecordRepository.AddAsync(messageRecord);
            var onlineClients = _hubContext.Clients.User(@event.UserId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION, websiteMessage);
        }
    }
}
