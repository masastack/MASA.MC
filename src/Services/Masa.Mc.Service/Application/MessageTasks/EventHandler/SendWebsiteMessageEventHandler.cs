// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWebsiteMessageEventHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly MessageRecordDomainService _messageRecordDomainService;

    public SendWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , ITemplateRenderer templateRenderer
        , MessageTemplateDomainService messageTemplateDomainService
        , MessageRecordDomainService messageRecordDomainService)
    {
        _hubContext = hubContext;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _templateRenderer = templateRenderer;
        _messageTemplateDomainService = messageTemplateDomainService;
        _messageRecordDomainService = messageRecordDomainService;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(SendWebsiteMessageEvent eto)
    {
        var taskHistory = eto.MessageTaskHistory;
        var userIds = new List<string>();
        int okCount = 0;
        int totalCount = taskHistory.ReceiverUsers.Count;
        if (taskHistory.IsTest || taskHistory.MessageTask.ReceiverType == ReceiverTypes.Assign)
        {
            foreach (var item in taskHistory.ReceiverUsers)
            {
                TemplateRenderer(eto.MessageData, item.Variables);
                var messageRecord = new MessageRecord(item.UserId, taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)), taskHistory.MessageTask.ExpectSendTime);
                _messageRecordDomainService.SetUserInfo(messageRecord, item);

                if (taskHistory.MessageTask.EntityType == MessageEntityTypes.Template)
                {
                    var perDayLimit = eto.MessageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(perDayLimit, item.UserId))
                    {
                        messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                messageRecord.SetResult(true, string.Empty);

                var linkUrl = eto.MessageData.GetDataValue<bool>(nameof(MessageTemplate.IsJump)) ? eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.JumpUrl)) : string.Empty;
                var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, item.UserId, eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Content)), linkUrl, DateTimeOffset.Now);
                await _messageRecordRepository.AddAsync(messageRecord);
                await _websiteMessageRepository.AddAsync(websiteMessage);

                userIds.Add(item.UserId.ToString());
                okCount++;
            }
        }
        taskHistory.SetResult((okCount == totalCount || taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast) ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();
        await _messageTaskHistoryRepository.UnitOfWork.CommitAsync();

        if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
        {
            var singalRGroup = _hubContext.Clients.Group("Global");
            await singalRGroup.SendAsync(SignalRMethodConsts.CHECK_NOTIFICATION);
        }
        if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Assign)
        {
            var onlineClients = _hubContext.Clients.Users(userIds);
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
        }
    }

    private async void TemplateRenderer(MessageData messageData, ExtraPropertyDictionary Variables)
    {
        messageData.SetDataValue(nameof(MessageTemplate.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), Variables));
        messageData.SetDataValue(nameof(MessageTemplate.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), Variables));
    }
}
