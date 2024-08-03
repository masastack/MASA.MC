// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.ChannelUserFinder.Provider.Auth;

public class AuthChannelUserFinder : IChannelUserFinder
{
    private readonly IAuthClient _authClient;
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

        if (receivers.Any(x => x.Type == MessageTaskReceiverTypes.User))
        {
            var userReceivers = await TransformUserReceivers(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.User));
            receiverUsers.AddRange(userReceivers);
        }

        if (receivers.Any(x => x.Type == MessageTaskReceiverTypes.Organization))
        {
            var orgIds = receivers.Where(x => x.Type == MessageTaskReceiverTypes.Organization).Select(x => x.SubjectId).Distinct();
            var orgReceivers = await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Organization, orgIds.ToList(), variables);
            receiverUsers.AddRange(orgReceivers);
        }

        if (receivers.Any(x => x.Type == MessageTaskReceiverTypes.Role))
        {
            var roleIds = receivers.Where(x => x.Type == MessageTaskReceiverTypes.Role).Select(x => x.SubjectId).Distinct();
            var roleReceivers = await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Role, roleIds.ToList(), variables);
            receiverUsers.AddRange(roleReceivers);
        }

        if (receivers.Any(x => x.Type == MessageTaskReceiverTypes.Team))
        {
            var teamIds = receivers.Where(x => x.Type == MessageTaskReceiverTypes.Team).Select(x => x.SubjectId).Distinct();
            var teamReceivers = await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Team, teamIds.ToList(), variables);
            receiverUsers.AddRange(teamReceivers);
        }

        if (receivers.Any(x => x.Type == MessageTaskReceiverTypes.Group))
        {
            var groupReceivers = await TransformGroupReceiversAsync(channel, variables, receivers.Where(x => x.Type == MessageTaskReceiverTypes.Group));
            receiverUsers.AddRange(groupReceivers);
        }

        return receiverUsers.Distinct();
    }

    public async Task<List<MessageReceiverUser>> TransformUserReceivers(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = new List<MessageReceiverUser>();
        var externalReceiverUsers = receivers.Where(x => !string.IsNullOrEmpty(x.ChannelUserIdentity)).Select(x => new MessageReceiverUser(x.SubjectId, x.ChannelUserIdentity, x.Variables.Any() ? x.Variables : variables)).ToList();
        receiverUsers.AddRange(externalReceiverUsers);

        var internalReceivers = receivers.Where(x => string.IsNullOrEmpty(x.ChannelUserIdentity)).ToList();

        if (!internalReceivers.Any())
        {
            return receiverUsers;
        }

        var internalReceiverUsers = await GetInternalReceiverUsers(channel, internalReceivers, variables);
        receiverUsers.AddRange(internalReceiverUsers);

        return receiverUsers;
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformGroupReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var receiverUsers = new List<MessageReceiverUser>();
        var receiverGroupIds = receivers.Select(x => x.SubjectId).Distinct();
        foreach (var receiverGroupId in receiverGroupIds)
        {
            var receiverGroup = await _receiverGroupRepository.FindAsync(x => x.Id == receiverGroupId);
            if (receiverGroup == null) continue;
            var typeGroups = receiverGroup.Items.GroupBy(x => x.Type).ToList();
            foreach (var items in typeGroups)
            {
                if (items.Key == ReceiverGroupItemTypes.User)
                {
                    var result = await GetInternalReceiverUsers(channel, items.Select(x => x.Receiver.SubjectId).ToList(), variables);
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

        if (channel.Type == ChannelType.Email || channel.Type == ChannelType.Sms || channel.Type == ChannelType.WebsiteMessage)
        {
            return authUsers.Select(x => new MessageReceiverUser(x.Id, channel.Type.GetChannelUserIdentity(x), variables)).ToList();
        }

        var receiverUsers = await GetInternalReceiverUsers(channel, authUsers.Select(x => x.Id).ToList(), variables);
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

    private async Task<List<MessageReceiverUser>> GetInternalReceiverUsers(AppChannel channel, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary variables)
    {
        var userIds = receivers.Select(x => x.SubjectId).ToList();
        var channelUserIdentitys = await GetChannelUserIdentitys(channel, userIds);

        return channelUserIdentitys.Select(x =>
        {
            var receiver = receivers.FirstOrDefault(r => r.SubjectId == x.Key);
            var receiverUser = new MessageReceiverUser(x.Key, x.Value, receiver?.Variables == null || !receiver.Variables.Any() ? variables : receiver.Variables);
            return receiverUser;
        }).ToList();
    }

    private async Task<List<MessageReceiverUser>> GetInternalReceiverUsers(AppChannel channel, List<Guid> userIds, ExtraPropertyDictionary variables)
    {
        var channelUserIdentitys = await GetChannelUserIdentitys(channel, userIds);

        return channelUserIdentitys.Select(x => new MessageReceiverUser(x.Key, x.Value, variables)).ToList();
    }

    private async Task<Dictionary<Guid, string>> GetChannelUserIdentitys(AppChannel channel, List<Guid> userIds)
    {
        var dic = new Dictionary<Guid, string>();

        if (!channel.Scheme.IsNullOrEmpty())
        {
            return await GetThirdPartyUserPushIdentitys(channel, userIds);
        }

        switch (channel.Type.Id)
        {
            case (int)ChannelTypes.App:
                var userSystemDatas = await _authClient.UserService.GetSystemListDataAsync<UserSystemData>(userIds, $"{MasaStackProject.MC.Name}:{channel.Code}");
                dic = userSystemDatas.ToDictionary(x => x.Key, x => x.Value?.ClientId ?? string.Empty);
                break;
            case (int)ChannelTypes.WeixinWork:
                dic = await _authClient.UserService.GetLdapUsersAccountAsync(userIds);
                break;
            case (int)ChannelTypes.Sms:
            case (int)ChannelTypes.Email:
                var authUsers = await _authClient.UserService.GetListByIdsAsync(userIds.ToArray());
                dic = authUsers.ToDictionary(x => x.Id, x => channel.Type.GetChannelUserIdentity(x));
                break;
            case (int)ChannelTypes.WebsiteMessage:
                dic = userIds.ToDictionary(x => x, x => x.ToString());
                break;
            default:
                break;
        }
        return userIds.ToDictionary(x => x, x => FindChannelUserIdentity(x, dic));
    }

    private async Task<Dictionary<Guid, string>> GetThirdPartyUserPushIdentitys(AppChannel channel, List<Guid> userIds)
    {
        var input = new GetThirdPartyUserFieldValueModel
        {
            Scheme = channel.Scheme,
            Field = channel.SchemeField,
            UserIds = userIds
        };
        var thirdPartyUserPushIdentitys = await _authClient.UserService.GetThirdPartyUserFieldValueAsync(input);
        return userIds.ToDictionary(x => x, x => FindChannelUserIdentity(x, thirdPartyUserPushIdentitys));
    }

    private string FindChannelUserIdentity(Guid userId, Dictionary<Guid, string> dic)
    {
        if (!dic.Any(y => y.Key == userId))
            return string.Empty;

        var userSystemData = dic.FirstOrDefault(y => y.Key == userId);
        return userSystemData.Value ?? string.Empty;
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
