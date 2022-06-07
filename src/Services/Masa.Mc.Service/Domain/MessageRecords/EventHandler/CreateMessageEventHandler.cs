// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class CreateMessageEventHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IReceiverGroupRepository _receiverGroupRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IAuthClient _authClient;
    public CreateMessageEventHandler(IChannelRepository channelRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IReceiverGroupRepository receiverGroupRepository
        , IDomainEventBus eventBus
        , IAuthClient authClient)
    {
        _channelRepository = channelRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _receiverGroupRepository = receiverGroupRepository;
        _eventBus = eventBus;
        _authClient = authClient;
    }

    [EventHandler(1)]
    public async void FillReceiverUsers(CreateMessageEvent eto)
    {
        eto.ReceiverUsers = new List<MessageReceiverUser>();
        eto.MessageTaskHistory = await _messageTaskHistoryRepository.FindAsync(x => x.Id == eto.MessageTaskHistoryId);
        if (eto.MessageTaskHistory == null || eto.MessageTaskHistory.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverUsers = eto.MessageTaskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User)
            .Select(x => new MessageReceiverUser
            {
                UserId = x.SubjectId,
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Variables = x.Variables.Count == 0 ? eto.MessageTaskHistory.Variables : x.Variables,
            })
            .ToList();
        eto.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(2)]
    public async Task FillDepartmentUsersAsync(CreateMessageEvent eto)
    {
        if (eto.MessageTaskHistory == null || eto.MessageTaskHistory.ReceiverType == ReceiverTypes.Broadcast) return;

        var orgIds = eto.MessageTaskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization).Select(x => x.SubjectId).Distinct();
        foreach (var orgId in orgIds)
        {
            var departmentUsers = await _authClient.UserService.GetListByDepartmentAsync(orgId);
            var receiverUsers = departmentUsers.Select(x => MapToMessageReceiverUser(x, eto.MessageTaskHistory.Variables))
            .ToList();
            eto.ReceiverUsers.AddRange(receiverUsers);
        }
    }

    [EventHandler(3)]
    public async Task FillRoleUsersAsync(CreateMessageEvent eto)
    {
        if (eto.MessageTaskHistory == null || eto.MessageTaskHistory.ReceiverType == ReceiverTypes.Broadcast) return;

        var roleIds = eto.MessageTaskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role).Select(x => x.SubjectId).Distinct();
        foreach (var roleId in roleIds)
        {
            var roleUsers = await _authClient.UserService.GetListByRoleAsync(roleId);
            var receiverUsers = roleUsers.Select(x => MapToMessageReceiverUser(x, eto.MessageTaskHistory.Variables))
            .ToList();
            eto.ReceiverUsers.AddRange(receiverUsers);
        }
    }

    [EventHandler(4)]
    public async Task FillTeamUsersAsync(CreateMessageEvent eto)
    {
        if (eto.MessageTaskHistory == null || eto.MessageTaskHistory.ReceiverType == ReceiverTypes.Broadcast) return;

        var teamIds = eto.MessageTaskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team).Select(x => x.SubjectId).Distinct();
        foreach (var teamId in teamIds)
        {
            var teamUsers = await _authClient.UserService.GetListByRoleAsync(teamId);
            var receiverUsers = teamUsers.Select(x => MapToMessageReceiverUser(x, eto.MessageTaskHistory.Variables))
            .ToList();
            eto.ReceiverUsers.AddRange(receiverUsers);
        }
    }

    [EventHandler(5)]
    public async Task FillReceiverGroupUsersAsync(CreateMessageEvent eto)
    {
        if (eto.MessageTaskHistory == null || eto.MessageTaskHistory.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverGroupIds = eto.MessageTaskHistory.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group).Select(x => x.SubjectId).Distinct();
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
                Variables = eto.MessageTaskHistory.Variables,
            })
            .ToList();
            eto.ReceiverUsers.AddRange(receiverGroupUsers);
        }
    }

    [EventHandler(6)]
    public async Task CheckSendAsync(CreateMessageEvent eto)
    {
        var receiverUsers = eto.ReceiverUsers;
        if (eto.MessageTaskHistory==null) return;
        eto.MessageTaskHistory.SetReceiverUsers(receiverUsers);
        eto.MessageTaskHistory.SetSending();
        await SendMessagesAsync(eto.ChannelId, eto.MessageData, eto.MessageTaskHistory);
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

    private MessageReceiverUser MapToMessageReceiverUser(StaffModel staff, ExtraPropertyDictionary variables)
    {
        return new MessageReceiverUser
        {
            UserId = staff.User.Id,
            DisplayName = staff.User.Name ?? staff.User.DisplayName ?? string.Empty,
            PhoneNumber = staff.User.PhoneNumber ?? string.Empty,
            Email = staff.User?.Email ?? string.Empty,
            Variables = variables,
        };
    }
}

