// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryWebsiteMessageEventHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly MessageTaskDomainService _taskDomainService;
    private readonly IWebsiteMessageRepository _repository;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly II18n<DefaultResource> _i18n;

    public RetryWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , IWebsiteMessageRepository repository
        , MessageTemplateDomainService messageTemplateDomainService
        , IMessageTemplateRepository templateRepository
        , II18n<DefaultResource> i18n)
    {
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _repository = repository;
        _messageTemplateDomainService = messageTemplateDomainService;
        _templateRepository = templateRepository;
        _i18n = i18n;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(RetryWebsiteMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null) return;

        var messageData = await _taskDomainService.GetMessageDataAsync(messageRecord.MessageTaskId, messageRecord.Variables);

        if (messageData.MessageType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _templateRepository.FindAsync(x => x.Id == messageRecord.MessageEntityId, false);
            if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageTemplate, messageRecord.ChannelUserIdentity))
            {
                messageRecord.SetResult(false, _i18n.T("DailySendingLimit"));
                await _messageRecordRepository.UpdateAsync(messageRecord);
                return;
            }
        }

        var websiteMessage = new WebsiteMessage(messageRecord.MessageTaskHistoryId, messageRecord.ChannelId, messageRecord.UserId, messageData.MessageContent.Title, messageData.MessageContent.Content, messageData.MessageContent.GetJumpUrl(), DateTimeOffset.Now, messageData.MessageContent.ExtraProperties);
        await _repository.AddAsync(websiteMessage);

        messageRecord.SetResult(true, string.Empty);

        await _messageRecordRepository.UpdateAsync(messageRecord);
        await _messageRecordRepository.UnitOfWork.SaveChangesAsync();
        await _messageRecordRepository.UnitOfWork.CommitAsync();

        if (messageRecord.Success == true)
        {
            var onlineClients = _hubContext.Clients.Users(messageRecord.UserId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
        }
    }
}
