// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

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
        if (taskHistory.ReceiverType == ReceiverTypes.Assign)
        {
            foreach (var item in taskHistory.ReceiverUsers)
            {
                TemplateRenderer(eto.MessageData, item.Variables);
                await _websiteMessageDomainService.CreateAsync(eto.MessageData, taskHistory, item);
                userIds.Add(item.UserId.ToString());
            }
        }
        taskHistory.SetComplete();
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();
        await _messageTaskHistoryRepository.UnitOfWork.CommitAsync();

        if (taskHistory.ReceiverType == ReceiverTypes.Broadcast)
        {
            var singalRGroup = _hubContext.Clients.Group("Global");
            await singalRGroup.SendAsync(SignalRMethodConsts.CHECK_NOTIFICATION);
        }
        if (taskHistory.ReceiverType == ReceiverTypes.Assign)
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
