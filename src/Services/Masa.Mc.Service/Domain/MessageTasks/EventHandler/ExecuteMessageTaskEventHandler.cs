﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class ExecuteMessageTaskEventHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly MessageTaskDomainService _domainService;
    public ExecuteMessageTaskEventHandler(IChannelRepository channelRepository
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IDomainEventBus eventBus
        , MessageTaskDomainService domainService)
    {
        _channelRepository = channelRepository;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(ExecuteMessageTaskEvent eto)
    {
        var history = await _messageTaskHistoryRepository.FindWaitSendAsync(eto.MessageTaskId, eto.IsTest);
        var messageData = await _domainService.GetMessageDataAsync(history.MessageTask.EntityType, history.MessageTask.EntityId, history.MessageTask.Variables);
        history.SetSending();
        if (!history.MessageTask.SendTime.HasValue)
        {
            history.MessageTask.SetSending();
        }
        await _messageTaskHistoryRepository.UpdateAsync(history);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();
        await SendMessagesAsync(history.MessageTask.ChannelId.Value, messageData, history);
    }

    private async Task SendMessagesAsync(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
    {
        var channel = await _channelRepository.FindAsync(x => x.Id == channelId);

        switch (channel.Type)
        {
            case ChannelTypes.Sms:
                await _eventBus.PublishAsync(new SendSmsMessageEvent(channelId, messageData, messageTaskHistory));
                break;
            case ChannelTypes.Email:
                await _eventBus.PublishAsync(new SendEmailMessageEvent(channelId, messageData, messageTaskHistory));
                break;
            case ChannelTypes.WebsiteMessage:
                await _eventBus.PublishAsync(new SendWebsiteMessageEvent(channelId, messageData, messageTaskHistory));
                break;
            default:
                break;
        }
    }
}
