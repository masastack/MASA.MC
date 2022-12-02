﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.ChannelUserFinder.Provider.Auth;

public class AuthChannelUserFinder : IChannelUserFinder
{
    private readonly IAuthClient _authClient;
    //Later, I will kill this IReceiverGroupRepository
    private readonly IReceiverGroupRepository _receiverGroupRepository;

    public AuthChannelUserFinder(IAuthClient authClient
        , IReceiverGroupRepository receiverGroupRepository)
    {
        _authClient = authClient;
        _receiverGroupRepository = receiverGroupRepository;
    }

    public async Task<IEnumerable<MessageReceiverUser>> GetReceiverUsersAsync(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = new List<MessageReceiverUser>();

        var userReceivers = await TransformUserReceivers(channelType, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.User));
        receiverUsers.AddRange(userReceivers);

        var orgReceivers = await TransformDepartmentReceiversAsync(channelType, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization));
        receiverUsers.AddRange(orgReceivers);

        var roleReceivers = await TransformRoleReceiversAsync(channelType, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role));
        receiverUsers.AddRange(roleReceivers);

        var teamReceivers = await TransformTeamReceiversAsync(channelType, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team));
        receiverUsers.AddRange(teamReceivers);

        var groupReceivers = await TransformGroupReceiversAsync(channelType, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group));
        receiverUsers.AddRange(groupReceivers);

        return receiverUsers;

    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformUserReceivers(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = receivers
            .Select(async x =>
            {
                var channelUserIdentity = await GetChannelUserIdentity(channelType, x.Receiver);
                return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, x.Variables.Count == 0 ? variables : x.Variables);
            })
            .ToList();

        var result = await Task.WhenAll(receiverUsers);
        return result;
    }

    private async Task<IEnumerable<MessageReceiverUser>> TransformDepartmentReceiversAsync(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var orgIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channelType, ReceiverGroupItemTypes.Organization, orgIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformRoleReceiversAsync(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var roleIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channelType, ReceiverGroupItemTypes.Role, roleIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformTeamReceiversAsync(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var teamIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channelType, ReceiverGroupItemTypes.Team, teamIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformGroupReceiversAsync(ChannelType channelType, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = new List<MessageReceiverUser>();
        var receiverGroupIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var typeGroups = receiverGroup.Items.GroupBy(x => x.Type).ToList();
            foreach (var items in typeGroups)
            {
                if (items.Key == ReceiverGroupItemTypes.User)
                {
                    var userList = items.Select(x =>
                    {
                        var channelUserIdentity = channelType.GetChannelUserIdentity(x.Receiver);
                        return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, variables);
                    });
                    receiverUsers.AddRange(userList);
                }
                else
                {
                    var subjectIds = items.Select(x => x.Receiver.SubjectId).Distinct().ToList();
                    var subjectUsers = await GetMessageReceiverUser(channelType, items.Key, subjectIds, variables);
                    receiverUsers.AddRange(subjectUsers);
                }
            }
        }

        return receiverUsers;
    }

    private async Task<List<MessageReceiverUser>> GetMessageReceiverUser(ChannelType channelType, ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var authUsers = await GetAuthUsers(type, subjectIds);
        return authUsers.Select(x =>
        {
            var receiver = new Receiver(x.UserId, x.DisplayName, x.Avatar, x.PhoneNumber, x.Email);
            var channelUserIdentity = channelType.GetChannelUserIdentity(receiver);
            return new MessageReceiverUser(x.UserId, channelUserIdentity, variables);
        }).ToList();
    }

    private async Task<List<StaffModel>> GetAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds)
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
        return userList;
    }

    private async Task<string> GetChannelUserIdentity(ChannelType channelType, Receiver receiver)
    {
        var channelUserIdentity = channelType.GetChannelUserIdentity(receiver);
        if (string.IsNullOrEmpty(channelUserIdentity))
        {
            var authUser = await _authClient.UserService.FindByIdAsync(receiver.SubjectId);
            if (authUser != null)
            {
                receiver = new Receiver(authUser.Id, authUser.DisplayName, authUser.Avatar, authUser.PhoneNumber ?? string.Empty, authUser.Email ?? string.Empty);
                channelUserIdentity = channelType.GetChannelUserIdentity(receiver);
            }
        }
        return channelUserIdentity;
    }
}
