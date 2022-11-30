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

    public RetryWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository
        , MessageTaskDomainService taskDomainService
        , IWebsiteMessageRepository repository
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
        _taskDomainService = taskDomainService;
        _repository = repository;
        _messageTemplateDomainService = messageTemplateDomainService;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(RetryWebsiteMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null)
        {
            return;
        }
        var messageData = await _taskDomainService.GetMessageDataAsync(messageRecord.MessageTaskId);

        if (messageData.MessageType == MessageEntityTypes.Template)
        {
            var perDayLimit = messageData.GetDataValue<long>(nameof(MessageTemplate.PerDayLimit));
            if (!await _messageTemplateDomainService.CheckSendUpperLimitAsync(messageRecord.MessageEntityId, perDayLimit, messageRecord.UserId))
            {
                messageRecord.SetResult(false, "The maximum number of times to send per day has been reached");
                await _messageRecordRepository.UpdateAsync(messageRecord);
                throw new UserFriendlyException("The maximum number of times to send per day has been reached");
            }
        }

        var linkUrl = messageData.GetDataValue<bool>(nameof(MessageContent.IsJump)) ? messageData.GetDataValue<string>(nameof(MessageContent.JumpUrl)) : string.Empty;
        var websiteMessage = new WebsiteMessage(messageRecord.ChannelId, messageRecord.UserId, messageData.GetDataValue<string>(nameof(MessageContent.Title)), messageData.GetDataValue<string>(nameof(MessageContent.Content)), linkUrl, DateTimeOffset.Now);
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
