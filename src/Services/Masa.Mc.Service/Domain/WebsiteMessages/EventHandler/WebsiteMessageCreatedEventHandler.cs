// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WebsiteMessageCreatedEventHandler
{
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _messageTaskDomainService;
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly IAuthClient _authClient;

    public WebsiteMessageCreatedEventHandler(IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService messageTaskDomainService
        , IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , IAuthClient authClient)
    {
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTaskDomainService = messageTaskDomainService;
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _authClient = authClient;
    }

    [EventHandler]
    public async Task HandleEvent(AddWebsiteMessageDomainEvent @event)
    {
        var currentUser = await _authClient.UserService.GetCurrentUserAsync();
        if (currentUser == null)
            return;

        var checkTime = @event.CheckTime;

        var taskHistorys = (await _messageTaskHistoryRepository.WithDetailsAsync()).Where(x => x.CompletionTime >= checkTime && x.MessageTask.ReceiverType == ReceiverTypes.Broadcast && x.Status == MessageTaskHistoryStatuses.Success).ToList();

        int okCount = 0;

        foreach (var taskHistory in taskHistorys)
        {
            if (taskHistory.MessageTask.ChannelType == ChannelType.App && !taskHistory.MessageTask.IsAppInWebsiteMessage())
                continue;

            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistory.MessageTask, taskHistory.MessageTask.Variables);
            var messageRecord = new MessageRecord(currentUser.Id, currentUser.Id.ToString(), taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, taskHistory.MessageTask.Variables, messageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageRecord.SetResult(true, string.Empty, taskHistory.SendTime);

            var websiteMessage = new WebsiteMessage(messageRecord.MessageTaskHistoryId, messageRecord.ChannelId, currentUser.Id, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.GetJumpUrl(), taskHistory.SendTime ?? DateTimeOffset.Now, messageData.MessageContent.ExtraProperties);
            await _messageRecordRepository.AddAsync(messageRecord);
            await _websiteMessageRepository.AddAsync(websiteMessage);

            okCount++;
        }

        if (okCount > 0)
        {
            var onlineClients = _hubContext.Clients.User(@event.UserId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
        }
    }
}
