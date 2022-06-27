﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IReceiverGroupRepository _receiverGroupRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IAuthClient _authClient;
    public ResolveMessageTaskEventHandler(IChannelRepository channelRepository
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IReceiverGroupRepository receiverGroupRepository
        , IDomainEventBus eventBus
        , IAuthClient authClient)
    {
        _channelRepository = channelRepository;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _receiverGroupRepository = receiverGroupRepository;
        _eventBus = eventBus;
        _authClient = authClient;
    }

    [EventHandler(1)]
    public async void FillReceiverUsers(ResolveMessageTaskEvent eto)
    {
        eto.MessageTask = (await _messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == eto.MessageTaskId);
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverUsers = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.User)
            .Select(x => new MessageReceiverUser
            {
                UserId = x.SubjectId,
                DisplayName = x.DisplayName,
                PhoneNumber = x.PhoneNumber,
                Email = x.Email,
                Variables = x.Variables.Count == 0 ? eto.MessageTask.Variables : x.Variables,
            })
            .ToList();
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(2)]
    public async Task FillDepartmentUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var orgIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Organization, orgIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(3)]
    public async Task FillRoleUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var roleIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Role, roleIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(4)]
    public async Task FillTeamUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var teamIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team).Select(x => x.SubjectId).Distinct();
        var receiverUsers = await ResolveAuthUsers(ReceiverGroupItemTypes.Team, teamIds.ToList(), eto.MessageTask.Variables);
        eto.MessageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(5)]
    public async Task FillReceiverGroupUsersAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null || eto.MessageTask.ReceiverType == ReceiverTypes.Broadcast) return;

        var receiverGroupIds = eto.MessageTask.Receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group).Select(x => x.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var typeGroups = receiverGroup.Items.GroupBy(x => x.Type).ToList();
            foreach (var items in typeGroups)
            {
                if (items.Key == ReceiverGroupItemTypes.User)
                {
                    var userList = items.Select(x => new MessageReceiverUser
                    {
                        UserId = x.SubjectId,
                        DisplayName = x.DisplayName,
                        PhoneNumber = x.PhoneNumber,
                        Email = x.Email,
                        Variables = eto.MessageTask.Variables,
                    });
                    eto.MessageTask.ReceiverUsers.AddRange(userList);
                }
                else
                {
                    var subjectIds = items.Select(x => x.SubjectId).Distinct().ToList();
                    var subjectUsers = await ResolveAuthUsers(items.Key, subjectIds, eto.MessageTask.Variables);
                    eto.MessageTask.ReceiverUsers.AddRange(subjectUsers);
                }
            }
        }
    }

    [EventHandler(6)]
    public async Task CheckSendAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.MessageTask == null) return;

        eto.MessageTask.SetSending();

        await _eventBus.PublishAsync(new ExecuteMessageTaskEvent(eto.MessageTask, eto.MessageTask.ReceiverUsers));
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

    private async Task<List<MessageReceiverUser>> ResolveAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var userList = new List<StaffModel>();
        switch (type)
        {
            case ReceiverGroupItemTypes.User:
                break;
            case ReceiverGroupItemTypes.Organization:
                foreach (var orgId in subjectIds)
                {
                    var departmentUsers = await _authClient.UserService.GetListByDepartmentAsync(orgId);
                    userList.AddRange(departmentUsers);
                }
                break;
            case ReceiverGroupItemTypes.Role:
                foreach (var roleId in subjectIds)
                {
                    var roleUsers = await _authClient.UserService.GetListByRoleAsync(roleId);
                    userList.AddRange(roleUsers);
                }
                break;
            case ReceiverGroupItemTypes.Team:
                foreach (var teamId in subjectIds)
                {
                    var teamUsers = await _authClient.UserService.GetListByTeamAsync(teamId);
                    userList.AddRange(teamUsers);
                }
                break;
            default:
                break;
        }
        return userList.Select(x => MapToMessageReceiverUser(x, variables))
            .ToList();
    }
}