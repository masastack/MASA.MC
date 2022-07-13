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
    private readonly MessageRecordDomainService _messageRecordDomainService;

    public WebsiteMessageCreatedEventHandler(IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTaskDomainService messageTaskDomainService
        , IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , MessageRecordDomainService messageRecordDomainService)
    {
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTaskDomainService = messageTaskDomainService;
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _messageRecordDomainService = messageRecordDomainService;
    }

    [EventHandler]
    public async Task HandleEvent(AddWebsiteMessageDomainEvent @event)
    {
        var checkStatus = new List<MessageTaskHistoryStatuses> { MessageTaskHistoryStatuses.Sending, MessageTaskHistoryStatuses.Success, MessageTaskHistoryStatuses.Fail, MessageTaskHistoryStatuses.PartialFailure };
        var checkTime = @event.CheckTime;
        var taskHistorys = (await _messageTaskHistoryRepository.WithDetailsAsync()).Where(x => x.CompletionTime >= checkTime && x.MessageTask.ReceiverType == ReceiverTypes.Broadcast && checkStatus.Contains(x.Status)).ToList();
        foreach (var taskHistory in taskHistorys)
        {
            var messageData = await _messageTaskDomainService.GetMessageDataAsync(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId, taskHistory.MessageTask.Variables);
            var receiverUser = new MessageReceiverUser()
            {
                UserId = Guid.Parse(TempCurrentUserConsts.ID),
                DisplayName = TempCurrentUserConsts.NAME,
                Account = TempCurrentUserConsts.USER_NAME,
                Email = TempCurrentUserConsts.EMAIL,
                PhoneNumber = TempCurrentUserConsts.PHONE_NUMBER
            };

            var messageRecord = new MessageRecord(receiverUser.UserId, taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, taskHistory.MessageTask.Variables, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), taskHistory.SendTime);
            _messageRecordDomainService.SetUserInfo(messageRecord, receiverUser);
            messageRecord.SetResult(true, string.Empty);

            var linkUrl = messageData.GetDataValue<bool>(nameof(MessageTemplate.IsJump)) ? messageData.GetDataValue<string>(nameof(MessageTemplate.JumpUrl)) : string.Empty;
            var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, receiverUser.UserId, messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), linkUrl, DateTimeOffset.Now);
            await _messageRecordRepository.AddAsync(messageRecord);
            await _websiteMessageRepository.AddAsync(websiteMessage);
        }
        var onlineClients = _hubContext.Clients.User(@event.UserId.ToString());
        await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
    }
}
