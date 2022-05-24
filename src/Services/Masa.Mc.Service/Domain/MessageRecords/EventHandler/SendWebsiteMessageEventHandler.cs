// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class SendWebsiteMessageEventHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly NotificationsSignalROptions _options;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;
    private readonly ITemplateRenderer _templateRenderer;

    public SendWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IOptions<NotificationsSignalROptions> options
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IWebsiteMessageRepository websiteMessageRepository
        , ITemplateRenderer templateRenderer)
    {
        _hubContext = hubContext;
        _options = options.Value;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _websiteMessageRepository = websiteMessageRepository;
        _templateRenderer = templateRenderer;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(SendWebsiteMessageEvent eto)
    {
        var taskHistory = eto.MessageTaskHistory;
        if (taskHistory.ReceiverType == ReceiverTypes.Broadcast)
        {
            TemplateRenderer(eto.MessageData, taskHistory.Variables);
            var singalRGroup = _hubContext.Clients.Group("Global");
            await singalRGroup.SendAsync(_options.MethodName, eto.MessageData.ExtraProperties);
        }
        if (taskHistory.ReceiverType == ReceiverTypes.Assign)
        {
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, eto.ChannelId, taskHistory.MessageTaskId, taskHistory.Id, item.Variables);
                SetUserInfo(messageRecord, item);
                TemplateRenderer(eto.MessageData, item.Variables);
                messageRecord.SetDataValue(nameof(MessageTemplate.Title), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)));
                try
                {
                    var onlineClients = _hubContext.Clients.User(item.UserId.ToString());
                    await onlineClients.SendAsync(_options.MethodName, eto.MessageData);
                    messageRecord.SetResult(true, string.Empty);
                }
                catch (Exception ex)
                {
                    messageRecord.SetResult(false, ex.Message);
                }
                await _messageRecordRepository.AddAsync(messageRecord);
                await _websiteMessageRepository.AddAsync(new WebsiteMessage(eto.ChannelId, item.UserId, eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Title)), eto.MessageData.GetDataValue<string>(nameof(MessageTemplate.Content)), taskHistory.SendTime.Value));
            }
        }
        taskHistory.SetComplete();
        await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
    }

    private async void TemplateRenderer(MessageData messageData, ExtraPropertyDictionary Variables)
    {
        messageData.SetDataValue(nameof(MessageTemplate.Title), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Title)), Variables));
        messageData.SetDataValue(nameof(MessageTemplate.Content), await _templateRenderer.RenderAsync(messageData.GetDataValue<string>(nameof(MessageTemplate.Content)), Variables));
    }

    private void SetUserInfo(MessageRecord messageRecord, MessageReceiverUser item)
    {
        messageRecord.SetDataValue(nameof(item.DisplayName), item.DisplayName);
        messageRecord.SetDataValue(nameof(item.Email), item.Email);
        messageRecord.SetDataValue(nameof(item.PhoneNumber), item.PhoneNumber);
    }
}
