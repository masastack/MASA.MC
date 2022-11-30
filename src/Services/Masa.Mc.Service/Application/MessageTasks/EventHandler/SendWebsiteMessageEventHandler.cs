// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendWebsiteMessageEventHandler
{
    private readonly IMcClient _mcClient;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;

    public SendWebsiteMessageEventHandler(IMcClient mcClient
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IMessageRecordRepository messageRecordRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , ITemplateRenderer templateRenderer
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _mcClient = mcClient;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageRecordRepository = messageRecordRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _templateRenderer = templateRenderer;
        _messageTemplateDomainService = messageTemplateDomainService;
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
                var messageRecord = new MessageRecord(item.Receiver.SubjectId, taskHistory.MessageTask.ChannelId.Value, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.GetDataValue<string>(nameof(MessageContent.Title)), taskHistory.SendTime);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                messageRecord.SetChannelUser(ChannelTypes.WebsiteMessage,item.Receiver.SubjectId.ToString());

                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    var perDayLimit = eto.MessageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageRecord.MessageEntityId, perDayLimit, item.Receiver.SubjectId))
                    {
                        messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                messageRecord.SetResult(true, string.Empty);

                var linkUrl = eto.MessageData.GetDataValue<bool>(nameof(MessageContent.IsJump)) ? eto.MessageData.GetDataValue<string>(nameof(MessageContent.JumpUrl)) : string.Empty;
                var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, item.Receiver.SubjectId, eto.MessageData.GetDataValue<string>(nameof(MessageContent.Title)), eto.MessageData.GetDataValue<string>(nameof(MessageContent.Content)), linkUrl, DateTimeOffset.Now);
                await _messageRecordRepository.AddAsync(messageRecord);
                await _websiteMessageRepository.AddAsync(websiteMessage);

                userIds.Add(item.Receiver.SubjectId.ToString());
                okCount++;
            }
        }
        taskHistory.SetResult((okCount == totalCount || taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast) ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();
        await _messageTaskHistoryRepository.UnitOfWork.CommitAsync();

        if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Broadcast)
        {
            await _mcClient.WebsiteMessageService.SendCheckNotificationAsync();
        }
        if (taskHistory.MessageTask.ReceiverType == ReceiverTypes.Assign)
        {
            await _mcClient.WebsiteMessageService.SendGetNotificationAsync(userIds);
        }
    }

    private async void TemplateRenderer(MessageData messageData, ExtraPropertyDictionary Variables)
    {
        messageData.SetDataValue(nameof(MessageContent.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageContent.Title)), Variables));
        messageData.SetDataValue(nameof(MessageContent.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageContent.Content)), Variables));
    }
}