// Copyright (c) MASA Stack All rights reserved.
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

    public async Task<IEnumerable<MessageReceiverUser>> GetReceiverUsersAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = new List<MessageReceiverUser>();

        var userReceivers = await TransformUserReceivers(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.User));
        receiverUsers.AddRange(userReceivers);

        var orgReceivers = await TransformDepartmentReceiversAsync(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization));
        receiverUsers.AddRange(orgReceivers);

        var roleReceivers = await TransformRoleReceiversAsync(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role));
        receiverUsers.AddRange(roleReceivers);

        var teamReceivers = await TransformTeamReceiversAsync(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team));
        receiverUsers.AddRange(teamReceivers);

        var groupReceivers = await TransformGroupReceiversAsync(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group));
        receiverUsers.AddRange(groupReceivers);

        return receiverUsers.DistinctBy(x => new { x.UserId, x.ChannelUserIdentity });

    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformUserReceivers(AppChannel Channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverList = receivers.ToList();

        if (Channel.Type == ChannelType.App)
        {
            var userIds = receiverList.Select(x => x.Receiver.SubjectId).ToList();
            var clientIds = await GetUserClientIds(Channel, userIds);

            return clientIds.Select(x =>
            {
                var receiver = receivers.FirstOrDefault(r => r.Receiver.SubjectId == x.UserId);
                var receiverUser = new MessageReceiverUser(x.UserId, x.ClientId, receiver?.Variables == null || !receiver.Variables.Any() ? variables : receiver.Variables);
                return receiverUser;
            });
        }

        var receiverUsers = await GetReceiverUsers(Channel, receiverList, variables);
        return receiverUsers;
    }

    private async Task<List<UserClient>> GetUserClientIds(AppChannel channel, List<Guid> userIds)
    {
        var userSystemDatas = await _authClient.UserService.GetSystemListDataAsync<UserSystemData>(userIds, $"{MasaStackProject.MC.Name}:{channel.Code}");
        var userClientIds = userIds.Select(x => new UserClient
        {
            UserId = x,
            ClientId = GetClientId(x, userSystemDatas)
        });
        return userClientIds.ToList();
    }

    private string GetClientId(Guid userId, Dictionary<Guid, UserSystemData?> userSystemDatas)
    {
        if (!userSystemDatas.Any(y => y.Key == userId))
            return string.Empty;

        var userSystemData = userSystemDatas.FirstOrDefault(y => y.Key == userId);
        return userSystemData.Value?.ClientId ?? string.Empty;
    }

    private async Task<IEnumerable<MessageReceiverUser>> TransformDepartmentReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var orgIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Organization, orgIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformRoleReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var roleIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Role, roleIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformTeamReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var teamIds = receivers.Select(x => x.Receiver.SubjectId).Distinct();
        return await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Team, teamIds.ToList(), variables);
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformGroupReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
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
                    var result = await GetReceiverUsers(channel, items.Select(x => x.Receiver).ToList(), variables);
                    receiverUsers.AddRange(result);
                }
                else
                {
                    var subjectIds = items.Select(x => x.Receiver.SubjectId).Distinct().ToList();
                    var subjectUsers = await GetMessageReceiverUser(channel, items.Key, subjectIds, variables);
                    receiverUsers.AddRange(subjectUsers);
                }
            }
        }

        return receiverUsers;
    }

    private async Task<IEnumerable<MessageReceiverUser>> GetMessageReceiverUser(AppChannel channel, ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var authUsers = await GetAuthUsers(type, subjectIds);

        if (channel.Type == ChannelType.App)
        {
            var userIds = authUsers.Select(x => x.Id).ToList();
            var clientIds = await GetUserClientIds(channel, userIds);

            return clientIds.Select(x => new MessageReceiverUser(x.UserId, x.ClientId, variables));
        }

        var receivers = authUsers.Select(x => new Receiver(x.Id, x.DisplayName, x.Avatar, x.PhoneNumber ?? string.Empty, x.Email ?? string.Empty)).ToList();
        var receiverUsers = await GetReceiverUsers(channel, receivers, variables);
        return receiverUsers;
    }

    private async Task<List<UserModel>> GetAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds)
    {
        var userList = new List<UserModel>();
        switch (type)
        {
            case ReceiverGroupItemTypes.User:
                break;
            case ReceiverGroupItemTypes.Organization:
                foreach (var orgId in subjectIds)
                {
                    var departmentUsers = await _authClient.UserService.GetListByDepartmentAsync(orgId);
                    userList.AddRange(GetUserModelByStaff(departmentUsers));
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
                    userList.AddRange(GetUserModelByStaff(teamUsers));
                }
                break;
            default:
                break;
        }
        return userList;
    }

    private async Task<List<MessageReceiverUser>> GetReceiverUsers(AppChannel channel, List<Receiver> receivers, ExtraPropertyDictionary variables)
    {
        var messageReceiverUsers = new List<MessageReceiverUser>();
        var userIds = new List<Guid>();

        foreach (var receiver in receivers)
        {
            var channelUserIdentity = channel.Type.GetChannelUserIdentity(receiver);
            if (string.IsNullOrEmpty(channelUserIdentity))
            {
                userIds.Add(receiver.SubjectId);
                continue;
            }

            messageReceiverUsers.Add(new MessageReceiverUser(receiver.SubjectId, channelUserIdentity, variables));
        }

        var authUsers = await _authClient.UserService.GetListByIdsAsync(userIds.ToArray());
        foreach (var userId in userIds)
        {
            var authUser = authUsers.FirstOrDefault(x => x.Id == userId);
            if (authUser == null)
            {
                messageReceiverUsers.Add(new MessageReceiverUser(userId, string.Empty, variables));
                continue;
            }

            var receiver = new Receiver(authUser.Id, authUser.DisplayName, authUser.Avatar, authUser.PhoneNumber ?? string.Empty, authUser.Email ?? string.Empty);
            var channelUserIdentity = channel.Type.GetChannelUserIdentity(receiver);
            messageReceiverUsers.Add(new MessageReceiverUser(receiver.SubjectId, channelUserIdentity, variables));
        }

        return messageReceiverUsers;
    }

    private async Task<List<MessageReceiverUser>> GetReceiverUsers(AppChannel channel, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary variables)
    {
        var messageReceiverUsers = new List<MessageReceiverUser>();
        var newReceivers = new List<MessageTaskReceiver>();

        foreach (var item in receivers)
        {
            var channelUserIdentity = channel.Type.GetChannelUserIdentity(item.Receiver);
            if (string.IsNullOrEmpty(channelUserIdentity))
            {
                newReceivers.Add(item);
                continue;
            }

            messageReceiverUsers.Add(new MessageReceiverUser(item.Receiver.SubjectId, channelUserIdentity, item.Variables.Any() ? item.Variables : variables));
        }

        var authUsers = await _authClient.UserService.GetListByIdsAsync(newReceivers.Select(x => x.Receiver.SubjectId).ToArray());
        foreach (var item in newReceivers)
        {
            var authUser = authUsers.FirstOrDefault(x => x.Id == item.Receiver.SubjectId);
            if (authUser == null)
            {
                messageReceiverUsers.Add(new MessageReceiverUser(item.Receiver.SubjectId, string.Empty, item.Variables.Any() ? item.Variables : variables));
                continue;
            }

            var receiver = new Receiver(authUser.Id, authUser.DisplayName, authUser.Avatar, authUser.PhoneNumber ?? string.Empty, authUser.Email ?? string.Empty);
            var channelUserIdentity = channel.Type.GetChannelUserIdentity(receiver);
            messageReceiverUsers.Add(new MessageReceiverUser(receiver.SubjectId, channelUserIdentity, item.Variables.Any() ? item.Variables : variables));
        }

        return messageReceiverUsers;
    }

    private List<UserModel> GetUserModelByStaff(List<StaffModel> staff)
    {
        return staff.Select(x => new UserModel
        {
            Id = x.UserId,
            DisplayName = x.DisplayName,
            Avatar = x.Avatar,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email
        }).ToList();
    }
}
