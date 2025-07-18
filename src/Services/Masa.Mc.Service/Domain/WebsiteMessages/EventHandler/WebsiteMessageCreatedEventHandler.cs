// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WebsiteMessageCreatedEventHandler
{
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTaskDomainService _messageTaskDomainService;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public WebsiteMessageCreatedEventHandler(IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService messageTaskDomainService
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , IUnitOfWork unitOfWork
        , IHubContext<NotificationsHub> hubContext)
    {
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTaskDomainService = messageTaskDomainService;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    [EventHandler]
    public async Task HandleEvent(AddWebsiteMessageDomainEvent @event)
    {
        var checkTime = @event.CheckTime;
        var userId = @event.UserId;

        var taskHistorys = (await _messageTaskHistoryRepository.WithDetailsAsync()).Where(x => x.MessageTask.ChannelType != ChannelType.WeixinWork && x.MessageTask.ChannelType != ChannelType.WeixinWorkWebhook  && x.CompletionTime >= checkTime && x.MessageTask.ReceiverType == ReceiverTypes.Broadcast && x.Status == MessageTaskHistoryStatuses.Success && !x.IsTest).ToList();

        int okCount = 0;

        foreach (var taskHistory in taskHistorys)
        {
            if (taskHistory.MessageTask.ChannelType == ChannelType.App && !taskHistory.MessageTask.IsAppInWebsiteMessage)
                continue;

            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistory.MessageTask, taskHistory.MessageTask.Variables);
            var messageRecord = new MessageRecord(userId, userId.ToString(), taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, taskHistory.MessageTask.Variables, messageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
            messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
            messageRecord.SetResult(true, string.Empty, taskHistory.SendTime);

            var websiteMessage = new WebsiteMessage(messageRecord.MessageTaskHistoryId, messageRecord.ChannelId, userId, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.GetJumpUrl(), taskHistory.SendTime ?? DateTimeOffset.UtcNow, messageData.MessageContent.ExtraProperties);
            await _messageRecordRepository.AddAsync(messageRecord);
            await _websiteMessageRepository.AddAsync(websiteMessage);

            okCount++;
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        if (okCount > 0)
        {
            var onlineClients = _hubContext.Clients.Users(userId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
        }
    }
}
