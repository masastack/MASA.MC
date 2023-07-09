// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.SignalR;

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWebsiteMessageEventHandler
{
    private readonly IMcClient _mcClient;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public SendWebsiteMessageEventHandler(IMcClient mcClient
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , ITemplateRenderer templateRenderer
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository templateRepository
        , II18n<DefaultResource> i18n
        , IHubContext<NotificationsHub> hubContext)
    {
        _mcClient = mcClient;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _templateRenderer = templateRenderer;
        _messageTemplateDomainService = messageTemplateDomainService;
        _templateRepository = templateRepository;
        _i18n = i18n;
        _hubContext = hubContext;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(SendWebsiteMessageEvent eto)
    {
        var taskHistory = eto.MessageTaskHistory;
        var userIds = new List<string>();
        int okCount = 0;
        int totalCount = taskHistory.ReceiverUsers.Count;

        var messageTemplate = await _templateRepository.FindAsync(x => x.Id == taskHistory.MessageTask.EntityId, false);
        var insertMessageRecords = new List<MessageRecord>();
        var insertWebsiteMessages = new List<WebsiteMessage>();

        if (taskHistory.IsTest || taskHistory.MessageTask.ReceiverType == ReceiverTypes.Assign)
        {
            foreach (var item in taskHistory.ReceiverUsers)
            {
                eto.MessageData.RenderContent(item.Variables);
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);

                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                        insertMessageRecords.Add(messageRecord);
                        continue;
                    }
                }

                messageRecord.SetResult(true, string.Empty);

                var websiteMessage = new WebsiteMessage(messageRecord.MessageTaskHistoryId, messageRecord.ChannelId, item.UserId, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now, eto.MessageData.MessageContent.ExtraProperties);
                insertMessageRecords.Add(messageRecord);
                insertWebsiteMessages.Add(websiteMessage);

                userIds.Add(item.ChannelUserIdentity);
                okCount++;
            }
        }

        await _messageRecordRepository.AddRangeAsync(insertMessageRecords);
        await _websiteMessageRepository.AddRangeAsync(insertWebsiteMessages);

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
}