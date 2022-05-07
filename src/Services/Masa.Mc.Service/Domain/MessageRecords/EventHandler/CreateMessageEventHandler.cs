// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

//did not finish
public class CreateMessageEventHandler
{
    private readonly IDomainEventBus _eventBus;
    private readonly IServiceProvider _serviceProvider;
    public CreateMessageEventHandler(
        IDomainEventBus eventBus
        , IServiceProvider serviceProvider)
    {
        _eventBus = eventBus;
        _serviceProvider = serviceProvider;
    }

    [EventHandler(1)]
    public async Task CheckReceiverUsersAsync(CreateMessageEvent eto)
    {
        //Auth did not do this temporarily
        var taskHistory = eto.MessageTaskHistory;
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
        eto.MessageTaskHistory.SetReceiverUsers(receiverUsers);
        await Task.CompletedTask;
    }

    [EventHandler(2)]
    public async Task SendMessagesAsync(CreateMessageEvent eto)
    {
        var unitOfWorkManager = _serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        await using var unitOfWork = unitOfWorkManager.CreateDbContext();
        var _channelRepository = unitOfWork.ServiceProvider.GetRequiredService<IChannelRepository>();
        var channel = await _channelRepository.FindAsync(x => x.Id == eto.ChannelId);
        switch (channel.Type)
        {
            case ChannelTypes.Sms:
                await _eventBus.PublishAsync(new SendSmsMessageEvent(eto.ChannelId, eto.MessageData, eto.MessageTaskHistory));
                break;
            case ChannelTypes.Email:
                await _eventBus.PublishAsync(new SendEmailMessageEvent(eto.ChannelId, eto.MessageData, eto.MessageTaskHistory));
                break;
            case ChannelTypes.WebsiteMessage:
                await _eventBus.PublishAsync(new SendWebsiteMessageEvent(eto.ChannelId, eto.MessageData, eto.MessageTaskHistory));
                break;
            default:
                break;
        }
    }
}

