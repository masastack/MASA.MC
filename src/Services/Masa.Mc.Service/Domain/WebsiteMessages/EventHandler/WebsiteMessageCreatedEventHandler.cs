// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WebsiteMessageCreatedEventHandler
{
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _messageTaskDomainService;
    private readonly WebsiteMessageDomainService _websiteMessageDomainService;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public WebsiteMessageCreatedEventHandler(IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService messageTaskDomainService
        , WebsiteMessageDomainService websiteMessageDomainService
        , IHubContext<NotificationsHub> hubContext)
    {
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTaskDomainService = messageTaskDomainService;
        _websiteMessageDomainService = websiteMessageDomainService;
        _hubContext = hubContext;
    }

    [EventHandler]
    public async Task HandleEvent(AddWebsiteMessageDomainEvent @event)
    {
        var checkStatus = new List<MessageTaskHistoryStatuses> { MessageTaskHistoryStatuses.Sending, MessageTaskHistoryStatuses.Completed };
        var checkTime = @event.CheckTime;
        var taskHistorys = (await _messageTaskHistoryRepository.WithDetailsAsync()).Where(x => x.SendTime >= checkTime && x.ReceiverType == ReceiverTypes.Broadcast && checkStatus.Contains(x.Status)).ToList();
        foreach (var taskHistory in taskHistorys)
        {
            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId, taskHistory.Variables);
            var receiverUser = new MessageReceiverUser()
            {
                UserId = Guid.Parse(TempCurrentUserConsts.ID),
                DisplayName = TempCurrentUserConsts.NAME,
                Email = TempCurrentUserConsts.EMAIL,
                PhoneNumber = TempCurrentUserConsts.PHONE_NUMBER
            };
            await _websiteMessageDomainService.CreateAsync(messageData, taskHistory, receiverUser);
        }
        var onlineClients = _hubContext.Clients.User(@event.UserId.ToString());
        await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
    }
}
