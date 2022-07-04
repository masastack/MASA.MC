// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWebsiteMessageEventHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly WebsiteMessageDomainService _websiteMessageDomainService;

    public SendWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , ITemplateRenderer templateRenderer
        , WebsiteMessageDomainService websiteMessageDomainService)
    {
        _hubContext = hubContext;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _templateRenderer = templateRenderer;
        _websiteMessageDomainService = websiteMessageDomainService;
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
                if (await _websiteMessageDomainService.CreateAsync(eto.MessageData, taskHistory, item))
                {
                    userIds.Add(item.UserId.ToString());
                    okCount++;
                }
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
