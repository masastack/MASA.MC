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
                var receiver = receivers.FirstOrDefault(r => r.Receiver.SubjectId == x.Key);
                var receiverUser = new MessageReceiverUser(x.Key, x.Value, receiver?.Variables == null || !receiver.Variables.Any() ? variables : receiver.Variables);
                return receiverUser;
            });
        }

        var receiverUsers = receiverList
            .Select(async x =>
            {
                var channelUserIdentity = await GetChannelUserIdentity(Channel, x.Receiver);
                return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, x.Variables.Count == 0 ? variables : x.Variables);
            })
            .ToList();

        var result = await Task.WhenAll(receiverUsers);
        return result;
    }

    private async Task<Dictionary<Guid, string>> GetUserClientIds(AppChannel channel, List<Guid> userIds)
    {
        var userSystemDatas = await _authClient.UserService.GetSystemListDataAsync<UserSystemData>(userIds, $"{MasaStackConsts.MC_SYSTEM_ID}:{channel.Code}");
        return userSystemDatas.ToDictionary(x => x.Key, x => x.Value.ClientId);
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
                    var userList = items.Select(async x =>
                    {
                        var channelUserIdentity = await GetChannelUserIdentity(channel, x.Receiver);
                        return new MessageReceiverUser(x.Receiver.SubjectId, channelUserIdentity, variables);
                    });
                    var result = await Task.WhenAll(userList);
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

            return clientIds.Select(x => new MessageReceiverUser(x.Key, x.Value, variables));
        }

        var receiverUsers = authUsers.Select(async x =>
        {
            var receiver = new Receiver(x.Id, x.DisplayName, x.Avatar, x.PhoneNumber ?? string.Empty, x.Email ?? string.Empty);
            var channelUserIdentity = await GetChannelUserIdentity(channel, receiver);
            return new MessageReceiverUser(x.Id, channelUserIdentity, variables);
        }).ToList();

        var result = await Task.WhenAll(receiverUsers);
        return result;
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

    private async Task<string> GetChannelUserIdentity(AppChannel Channel, Receiver receiver)
    {
        var channelUserIdentity = Channel.Type.GetChannelUserIdentity(receiver);
        if (string.IsNullOrEmpty(channelUserIdentity))
        {
            if (Channel.Type == ChannelType.App)
            {
                var userSystemData = await _authClient.UserService.GetSystemDataAsync<UserSystemData>(receiver.SubjectId, $"{MasaStackConsts.MC_SYSTEM_ID}:{Channel.Code}");
                return userSystemData?.ClientId ?? string.Empty;
            }

            var authUser = await _authClient.UserService.GetByIdAsync(receiver.SubjectId);
            if (authUser != null)
            {
                receiver = new Receiver(authUser.Id, authUser.DisplayName, authUser.Avatar, authUser.PhoneNumber ?? string.Empty, authUser.Email ?? string.Empty);
                channelUserIdentity = Channel.Type.GetChannelUserIdentity(receiver);
            }
        }
        return channelUserIdentity;
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
