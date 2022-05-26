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
        var checkTime = @event.CheckTime;
        var taskHistorys = (await _messageTaskHistoryRepository.WithDetailsAsync()).Where(x => x.SendTime >= checkTime && x.ReceiverType == ReceiverTypes.Broadcast && checkStatus.Contains(x.Status)).ToList();
        foreach (var taskHistory in taskHistorys)
        {
            if (await _messageRecordRepository.FindAsync(x => x.MessageTaskHistoryId == taskHistory.Id && x.UserId == Guid.Parse(TempCurrentUserConsts.ID)) != null) continue;
            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId, taskHistory.Variables);

            var websiteMessage = new WebsiteMessage(taskHistory.MessageTask.ChannelId, @event.UserId, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), taskHistory.SendTime.Value);
            await _repository.AddAsync(websiteMessage);

            var messageRecord = new MessageRecord(@event.UserId, websiteMessage.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, taskHistory.Variables);
            SetExtraProperties(messageRecord, messageData);

            messageRecord.SetResult(true, string.Empty);
            await _messageRecordRepository.AddAsync(messageRecord);
        }
        var onlineClients = _hubContext.Clients.User(@event.UserId.ToString());
        await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
    }

    private void SetExtraProperties(MessageRecord messageRecord, MessageData messageData)
    {
        messageRecord.SetDataValue(nameof(MessageReceiverUser.DisplayName), TempCurrentUserConsts.NAME);
        messageRecord.SetDataValue(nameof(MessageReceiverUser.Email), TempCurrentUserConsts.EMAIL);
        messageRecord.SetDataValue(nameof(MessageReceiverUser.PhoneNumber), TempCurrentUserConsts.PHONE_NUMBER);
        messageRecord.SetDataValue(nameof(MessageTemplate.Title), messageData.GetDataValue<string>(nameof(MessageTemplate.Title)));
    }
}
