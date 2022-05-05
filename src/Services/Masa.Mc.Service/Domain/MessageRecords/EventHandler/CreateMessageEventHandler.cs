// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class CreateMessageEventHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public CreateMessageEventHandler(IChannelRepository channelRepository
        , IDomainEventBus eventBus
        , IMessageTaskRepository messageTaskRepository
        , IMessageRecordRepository messageRecordRepository)
    {
        _channelRepository = channelRepository;
        _eventBus = eventBus;
        _messageTaskRepository = messageTaskRepository;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler(1)]
    public async Task CheckUsersAsync(CreateMessageEvent @event)
    {
        //Auth did not do this temporarily
        var task = await _messageTaskRepository.FindAsync(x => x.Id == @event.MessageTaskId);
        var history = task.Historys.FirstOrDefault(x => x.Id == @event.MessageTaskHistoryId);
        var userIds = history.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User && !string.IsNullOrEmpty(x.SubjectId)).Select(x => new Guid(x.SubjectId));
        @event.UserIds = userIds;
    }

    [EventHandler(2)]
    public async Task CreateMessagesAsync(CreateMessageEvent @event)
    {
        var list = new List<MessageRecord>();
        foreach (var item in @event.UserIds)
        {
            list.Add(new MessageRecord(item, @event.ChannelId, @event.MessageTaskId, @event.MessageTaskHistoryId));
        }
        await _messageRecordRepository.AddRangeAsync(list);
    }

    [EventHandler(3)]
    public async Task SendMessagesAsync(CreateMessageEvent @event)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == @event.ChannelId);
        switch (channel.Type)
        {
            case ChannelTypes.Sms:
                await _eventBus.PublishAsync(new CreateSmsMessageEvent(@event.ChannelId, @event.MessageTaskId, @event.MessageTaskHistoryId, @event.UserIds));
                break;
            case ChannelTypes.Email:
                await _eventBus.PublishAsync(new CreateEmailMessageEvent(@event.ChannelId, @event.MessageTaskId, @event.MessageTaskHistoryId, @event.UserIds));
                break;
            case ChannelTypes.WebsiteMessage:
                await _eventBus.PublishAsync(new CreateWebsiteMessageEvent(@event.ChannelId, @event.MessageTaskId, @event.MessageTaskHistoryId, @event.UserIds));
                break;
            default:
                break;
        }
    }
}

