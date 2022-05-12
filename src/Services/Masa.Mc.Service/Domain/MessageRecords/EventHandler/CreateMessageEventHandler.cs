// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

//did not finish
public class CreateMessageEventHandler
{
    private readonly IServiceProvider _serviceProvider;
    public CreateMessageEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [EventHandler(1)]
    public async Task CheckReceiverUsersAsync(CreateMessageEvent eto)
    {
        //Auth did not do this temporarily
        var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var unitOfWork = unitOfWorkManager.CreateDbContext();
        var _messageTaskHistoryRepository = unitOfWork.ServiceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var _receiverGroupRepository = unitOfWork.ServiceProvider.GetRequiredService<IReceiverGroupRepository>();
        var taskHistory = await _messageTaskHistoryRepository.FindAsync(x => x.Id == eto.MessageTaskHistoryId);
        var receiverUsers = taskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User)
            .Select(x => new MessageReceiverUser
            {
                UserId = x.SubjectId,
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Variables = taskHistory.Variables,
            })
            .ToList();
        var receiverGroupIds = taskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group).Select(x => x.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var receiverGroupUsers = receiverGroup.Items.Where(x => x.Type == ReceiverGroupItemTypes.User)
            .Select(x => new MessageReceiverUser
            {
                UserId = x.SubjectId,
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Variables = taskHistory.Variables,
            })
            .ToList();
            receiverUsers.AddRange(receiverGroupUsers);
        }
        taskHistory.SetReceiverUsers(receiverUsers);
        taskHistory.SetSending();
        await SendMessagesAsync(unitOfWork.ServiceProvider, eto.ChannelId, eto.MessageData, taskHistory);
    }

    private async Task SendMessagesAsync(IServiceProvider serviceProvider, Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
    {
        var unitOfWork = serviceProvider.GetService<IUnitOfWork>();
        if (unitOfWork != null)
            unitOfWork.UseTransaction = false;
        var _channelRepository = serviceProvider.GetRequiredService<IChannelRepository>();
        var _eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
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

