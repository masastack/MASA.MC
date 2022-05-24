// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WebsiteMessageCreatedEventHandler
{
    private readonly IWebsiteMessageRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    public WebsiteMessageCreatedEventHandler(IWebsiteMessageRepository repository, IMessageTaskHistoryRepository messageTaskHistoryRepository)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
    }

    [EventHandler]
    public async Task HandleEvent(WebsiteMessageCreatedDomainEvent @event)
    {
        var checkStatus = new List<MessageTaskHistoryStatuses> { MessageTaskHistoryStatuses.Sending, MessageTaskHistoryStatuses.Completed };
        var taskHistorys = await _messageTaskHistoryRepository.GetListAsync(x => x.SendTime >= @event.CheckTime && x.ReceiverType == ReceiverTypes.Broadcast && checkStatus.Contains(x.Status));
        foreach (var taskHistory in taskHistorys)
        {
            var taskHistoryDetail = await _messageTaskHistoryRepository.FindAsync(x => x.Id == taskHistory.Id, true);
            if (taskHistoryDetail == null) continue;
            await _repository.AddAsync(new WebsiteMessage(taskHistoryDetail.MessageTask.ChannelId, @event.UserId, taskHistoryDetail.MessageTask.DisplayName, "", taskHistoryDetail.SendTime.Value));
        }
    }
}
