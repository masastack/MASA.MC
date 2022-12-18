// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class SendAppMessageEventHandler
{
    private readonly IAppNotificationAsyncLocal _appNotificationAsyncLocal;
    private readonly IAppNotificationSender _appNotificationSender;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly ILogger<SendEmailMessageEventHandler> _logger;
    private readonly IMessageTemplateRepository _repository;
    private readonly IWebsiteMessageRepository _websiteMessageRepository;

    public SendAppMessageEventHandler(IAppNotificationAsyncLocal appNotificationAsyncLocal
        , IAppNotificationSender appNotificationSender
        , ITemplateRenderer templateRenderer
        , IChannelRepository channelRepository
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , MessageTemplateDomainService messageTemplateDomainService
        , ILogger<SendEmailMessageEventHandler> logger
        , IMessageTemplateRepository repository
        , IWebsiteMessageRepository websiteMessageRepository)
    {
        _appNotificationAsyncLocal = appNotificationAsyncLocal;
        _appNotificationSender = appNotificationSender;
        _templateRenderer = templateRenderer;
        _channelRepository = channelRepository;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _logger = logger;
        _repository = repository;
        _websiteMessageRepository = websiteMessageRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendAppMessageEvent eto)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        var options = new GetuiOptions
        {
            AppID = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppID)),
            AppKey = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppKey)),
            AppSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.AppSecret)),
            MasterSecret = channel.ExtraProperties.GetProperty<string>(nameof(AppChannelOptions.MasterSecret))
        };
        using (_appNotificationAsyncLocal.Change(options))
        {
            var taskHistory = eto.MessageTaskHistory;
            int okCount = 0;
            int totalCount = taskHistory.ReceiverUsers.Count;
            foreach (var item in taskHistory.ReceiverUsers)
            {
                var messageRecord = new MessageRecord(item.UserId, item.ChannelUserIdentity, channel.Id, taskHistory.MessageTaskId, taskHistory.Id, item.Variables, eto.MessageData.MessageContent.Title, taskHistory.SendTime, taskHistory.MessageTask.SystemId);
                messageRecord.SetMessageEntity(taskHistory.MessageTask.EntityType, taskHistory.MessageTask.EntityId);
                eto.MessageData.RenderContent(item.Variables);
                if (eto.MessageData.MessageType == MessageEntityTypes.Template)
                {
                    var messageTemplate = await _repository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
                    if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
                    {
                        messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                        await _messageRecordRepository.AddAsync(messageRecord);
                        continue;
                    }
                }

                try
                {
                    var appChannel = channel.Type as ChannelType.AppsChannel;
                    var transmissionContent = appChannel.GetMessageTransmissionContent(eto.MessageData.MessageContent);
                    var response = await _appNotificationSender.SendAsync(new AppMessage(item.ChannelUserIdentity, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, transmissionContent));
                    if (response.Success)
                    {
                        messageRecord.SetResult(true, string.Empty);

                        if (eto.MessageData.MessageContent.ExtraProperties.GetProperty<bool>("IsWebsiteMessage"))
                        {
                            var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, item.UserId, eto.MessageData.MessageContent.Title, eto.MessageData.MessageContent.Content, eto.MessageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now);
                            await _websiteMessageRepository.AddAsync(websiteMessage);
                        }
                        okCount++;
                    }
                    else
                    {
                        messageRecord.SetResult(false, response.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SendAppMessageEventHandler");
                    messageRecord.SetResult(false, ex.Message);
                }

                await _messageRecordRepository.AddAsync(messageRecord);
            }
            taskHistory.SetResult(okCount == totalCount ? MessageTaskHistoryStatuses.Success : (okCount > 0 ? MessageTaskHistoryStatuses.PartialFailure : MessageTaskHistoryStatuses.Fail));
            await _messageTaskHistoryRepository.UpdateAsync(taskHistory);
        }
    }
}
