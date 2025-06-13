// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.ChannelUserFinder.Provider.Auth;

public class AuthChannelUserFinder : IChannelUserFinder
{
    private readonly IAuthClient _authClient;
    private readonly IReceiverGroupRepository _receiverGroupRepository;
    private readonly IAppDeviceTokenRepository _appDeviceTokenRepository;

    public AuthChannelUserFinder(IAuthClient authClient, IReceiverGroupRepository receiverGroupRepository, IAppDeviceTokenRepository appDeviceTokenRepository)
    {
        _authClient = authClient;
        _receiverGroupRepository = receiverGroupRepository;
        _appDeviceTokenRepository = appDeviceTokenRepository;
    }

    public async Task<IEnumerable<MessageReceiverUser>> GetReceiverUsersAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var groupedReceivers = receivers.GroupBy(r => r.Type).ToDictionary(g => g.Key, g => g.ToList());
        var result = new List<MessageReceiverUser>();

        if (groupedReceivers.TryGetValue(MessageTaskReceiverTypes.User, out var userReceivers))
            result.AddRange(await TransformUserReceivers(channel, variables, userReceivers));

        if (groupedReceivers.TryGetValue(MessageTaskReceiverTypes.Organization, out var orgReceivers))
            result.AddRange(await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Organization, ExtractSubjectIds(orgReceivers), variables));

        if (groupedReceivers.TryGetValue(MessageTaskReceiverTypes.Role, out var roleReceivers))
            result.AddRange(await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Role, ExtractSubjectIds(roleReceivers), variables));

        if (groupedReceivers.TryGetValue(MessageTaskReceiverTypes.Team, out var teamReceivers))
            result.AddRange(await GetMessageReceiverUser(channel, ReceiverGroupItemTypes.Team, ExtractSubjectIds(teamReceivers), variables));

        if (groupedReceivers.TryGetValue(MessageTaskReceiverTypes.Group, out var groupReceivers))
            result.AddRange(await TransformGroupReceiversAsync(channel, variables, groupReceivers));

        return result.Distinct();
    }

    private static List<Guid> ExtractSubjectIds(IEnumerable<MessageTaskReceiver> receivers) =>
        receivers.Select(r => r.SubjectId).Distinct().ToList();

    public async Task<List<MessageReceiverUser>> TransformUserReceivers(AppChannel channel, ExtraPropertyDictionary variables, IReadOnlyCollection<MessageTaskReceiver> receivers)
    {
        var directUsers = receivers
            .Where(r => !string.IsNullOrEmpty(r.ChannelUserIdentity))
            .Select(r => new MessageReceiverUser(
                r.SubjectId,
                r.ChannelUserIdentity,
                r.Variables.Any() ? r.Variables : variables
            ))
            .ToList();

        var internalReceivers = receivers.Where(r => string.IsNullOrEmpty(r.ChannelUserIdentity)).ToList();
        if (internalReceivers.Count > 0)
        {
            directUsers.AddRange(await GetInternalReceiverUsers(channel, internalReceivers, variables));
        }

        return directUsers;
    }

    public async Task<IEnumerable<MessageReceiverUser>> TransformGroupReceiversAsync(AppChannel channel, ExtraPropertyDictionary variables, IEnumerable<MessageTaskReceiver> receivers)
    {
        var groupIds = ExtractSubjectIds(receivers);
        var groups = await _receiverGroupRepository.GetListAsync(g => groupIds.Contains(g.Id));
        var result = new List<MessageReceiverUser>();

        foreach (var group in groups)
        {
            foreach (var itemGroup in group.Items.GroupBy(i => i.Type))
            {
                var subjectIds = itemGroup.Select(i => i.SubjectId).Distinct().ToList();

                if (itemGroup.Key == ReceiverGroupItemTypes.User)
                    result.AddRange(await GetInternalReceiverUsers(channel, subjectIds, variables));
                else
                    result.AddRange(await GetMessageReceiverUser(channel, itemGroup.Key, subjectIds, variables));
            }
        }

        return result;
    }

    private async Task<IEnumerable<MessageReceiverUser>> GetMessageReceiverUser(AppChannel channel, ReceiverGroupItemTypes type, List<Guid> subjectIds, ExtraPropertyDictionary variables)
    {
        var users = await GetAuthUsers(type, subjectIds);

        if (channel.Type == ChannelType.Email || channel.Type == ChannelType.Sms || channel.Type == ChannelType.WebsiteMessage)
            return users.Select(u => new MessageReceiverUser(u.Id, channel.Type.GetChannelUserIdentity(u), variables));

        return await GetInternalReceiverUsers(channel, users.Select(u => u.Id).ToList(), variables);
    }

    private async Task<List<UserModel>> GetAuthUsers(ReceiverGroupItemTypes type, List<Guid> subjectIds)
    {
        return type switch
        {
            ReceiverGroupItemTypes.Organization => await GetOrganizationUsers(subjectIds),
            ReceiverGroupItemTypes.Role => await GetRoleUsers(subjectIds),
            ReceiverGroupItemTypes.Team => await GetTeamUsers(subjectIds),
            _ => new List<UserModel>()
        };
    }

    private async Task<List<UserModel>> GetOrganizationUsers(List<Guid> orgIds)
    {
        var results = await Task.WhenAll(orgIds.Select(_authClient.UserService.GetListByDepartmentAsync));
        return results.SelectMany(GetUserModelByStaff).ToList();
    }

    private async Task<List<UserModel>> GetRoleUsers(List<Guid> roleIds)
    {
        var results = await Task.WhenAll(roleIds.Select(_authClient.UserService.GetListByRoleAsync));
        return results.SelectMany(x => x).ToList();
    }

    private async Task<List<UserModel>> GetTeamUsers(List<Guid> teamIds)
    {
        var results = await Task.WhenAll(teamIds.Select(_authClient.UserService.GetListByTeamAsync));
        return results.SelectMany(GetUserModelByStaff).ToList();
    }

    private async Task<List<MessageReceiverUser>> GetInternalReceiverUsers(AppChannel channel, IReadOnlyCollection<MessageTaskReceiver> receivers, ExtraPropertyDictionary variables)
    {
        if (channel.Type == ChannelType.App)
        {
            return await GetAppChannelReceiverUser(receivers, channel.Id, variables);
        }

        var userIds = receivers.Select(r => r.SubjectId).ToList();
        var identities = await GetChannelUserIdentitys(channel, userIds);
        var receiverVars = receivers.ToDictionary(r => r.SubjectId, r => r.Variables);

        return identities.Select(kv => new MessageReceiverUser(
            kv.Key,
            kv.Value,
            receiverVars.TryGetValue(kv.Key, out var vars) && vars.Any()
                ? vars
                : variables
        )).ToList();
    }

    private async Task<List<MessageReceiverUser>> GetInternalReceiverUsers(AppChannel channel, List<Guid> userIds, ExtraPropertyDictionary variables)
    {
        if (channel.Type == ChannelType.App)
        {
            return await GetAppChannelReceiverUser(userIds, channel.Id, variables);
        }

        var identities = await GetChannelUserIdentitys(channel, userIds);
        return identities.Select(kv => new MessageReceiverUser(kv.Key, kv.Value, variables)).ToList();
    }

    private async Task<Dictionary<Guid, string>> GetChannelUserIdentitys(AppChannel channel, List<Guid> userIds)
    {
        if (!string.IsNullOrEmpty(channel.Scheme))
            return await GetThirdPartyUserPushIdentitys(channel, userIds);

        return channel.Type.Id switch
        {
            //(int)ChannelTypes.App => await GetAppChannelUserIdentities(userIds, channel),
            (int)ChannelTypes.WeixinWork or (int)ChannelTypes.WeixinWorkWebhook => await _authClient.UserService.GetLdapUsersAccountAsync(userIds),
            (int)ChannelTypes.Sms or (int)ChannelTypes.Email => await GetEmailOrSmsUserIdentities(userIds, channel),
            (int)ChannelTypes.WebsiteMessage => userIds.ToDictionary(id => id, id => id.ToString()),
            _ => new Dictionary<Guid, string>()
        };
    }

    private async Task<List<MessageReceiverUser>> GetAppChannelReceiverUser(IReadOnlyCollection<MessageTaskReceiver> receivers, Guid channelId, ExtraPropertyDictionary variables)
    {
        var userIds = receivers.Select(r => r.SubjectId).ToList();
        var receiverVars = receivers.ToDictionary(r => r.SubjectId, r => r.Variables);

        var appDeviceTokens = await _appDeviceTokenRepository.GetListAsync(x => x.ChannelId == channelId && userIds.Contains(x.UserId));


        return appDeviceTokens.Select(x => new MessageReceiverUser(x.UserId, x.DeviceToken, receiverVars.TryGetValue(x.UserId, out var vars) && vars.Any()
                ? vars
                : variables, x.Platform.ToString())).ToList();
    }

    private async Task<List<MessageReceiverUser>> GetAppChannelReceiverUser(List<Guid> userIds, Guid channelId, ExtraPropertyDictionary variables)
    {
        var appDeviceTokens = await _appDeviceTokenRepository.GetListAsync(x => x.ChannelId == channelId && userIds.Contains(x.UserId));

        return appDeviceTokens.Select(x => new MessageReceiverUser(x.UserId, x.DeviceToken, variables, x.Platform.ToString())).ToList();
    }

    private async Task<Dictionary<Guid, string>> GetEmailOrSmsUserIdentities(List<Guid> userIds, AppChannel channel)
    {
        var users = await _authClient.UserService.GetListByIdsAsync(userIds.ToArray());
        return users.ToDictionary(u => u.Id, u => channel.Type.GetChannelUserIdentity(u));
    }

    private async Task<Dictionary<Guid, string>> GetThirdPartyUserPushIdentitys(AppChannel channel, List<Guid> userIds)
    {
        var input = new GetThirdPartyUserFieldValueModel
        {
            Scheme = channel.Scheme,
            Field = channel.SchemeField,
            UserIds = userIds
        };

        var result = await _authClient.UserService.GetThirdPartyUserFieldValueAsync(input);
        return userIds.ToDictionary(id => id, id => result.GetValueOrDefault(id, string.Empty));
    }

    private List<UserModel> GetUserModelByStaff(List<StaffModel> staff) =>
        staff.Select(s => new UserModel
        {
            Id = s.UserId,
            DisplayName = s.DisplayName,
            Avatar = s.Avatar,
            PhoneNumber = s.PhoneNumber,
            Email = s.Email
        }).ToList();
}