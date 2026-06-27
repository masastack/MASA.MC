// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

public class UnsubscriptionCommandHandler
{
    private readonly UnsubscriptionDomainService _unsubscriptionDomainService;
    private readonly IUserContext _userContext;
    private readonly IAuthClient _authClient;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public UnsubscriptionCommandHandler(
        UnsubscriptionDomainService unsubscriptionDomainService,
        IUserContext userContext,
        IAuthClient authClient,
        IChannelRepository channelRepository,
        IMessageTemplateRepository messageTemplateRepository)
    {
        _unsubscriptionDomainService = unsubscriptionDomainService;
        _userContext = userContext;
        _authClient = authClient;
        _channelRepository = channelRepository;
        _messageTemplateRepository = messageTemplateRepository;
    }

    [EventHandler]
    public async Task AddChannelUserIdentityToBlacklistAsync(AddChannelUserIdentityToUnsubscriptionBlacklistCommand command)
    {
        var channel = await ResolveChannelAsync(command.Input.ChannelCode);
        var templateId = await ResolveTemplateIdAsync(channel.Id, command.Input.TemplateCode);

        var channelType = (ChannelTypes)Enum.Parse(typeof(ChannelTypes), channel.Type.Id.ToString());
        var (targetUserId, targetChannelUserIdentity) = await ResolveTargetUserAsync(
            channelType,
            command.Input.UserId,
            command.Input.ChannelUserIdentity);
        await _unsubscriptionDomainService.AddChannelUserIdentityToBlacklistAsync(
            targetUserId,
            channel.Id,
            channelType,
            channel.Provider,
            targetChannelUserIdentity,
            templateId,
            command.Input.Reason);
    }

    [EventHandler]
    public async Task RemoveChannelUserIdentityFromBlacklistAsync(RemoveChannelUserIdentityFromUnsubscriptionBlacklistCommand command)
    {
        var channel = await ResolveChannelAsync(command.Input.ChannelCode);
        var templateId = await ResolveTemplateIdAsync(channel.Id, command.Input.TemplateCode);

        var channelType = (ChannelTypes)Enum.Parse(typeof(ChannelTypes), channel.Type.Id.ToString());
        await _unsubscriptionDomainService.RemoveChannelUserIdentityFromBlacklistAsync(
            _userContext.GetUserId<Guid>(),
            channel.Id,
            channelType,
            channel.Provider,
            command.Input.ChannelUserIdentity,
            templateId,
            command.Input.Reason);
    }

    private async Task<Channel> ResolveChannelAsync(string channelCode)
    {
        var normalizedChannelCode = channelCode?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedChannelCode))
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_REQUIRED);
        }

        var channel = await _channelRepository.FindAsync(x => x.Code == normalizedChannelCode);
        MasaArgumentException.ThrowIfNull(channel, nameof(Channel));
        return channel!;
    }

    private async Task<Guid?> ResolveTemplateIdAsync(Guid channelId, string templateCode)
    {
        var normalizedTemplateCode = templateCode?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedTemplateCode))
        {
            return null;
        }

        var template = await _messageTemplateRepository.FindAsync(x =>
            x.ChannelId == channelId &&
            x.Code == normalizedTemplateCode);
        if (template is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TEMPLATE_NOT_EXIST);
        }

        return template.Id;
    }

    private async Task<Guid> ResolveTargetUserIdAsync(ChannelTypes channelType, string channelUserIdentity)
    {
        var normalizedChannelUserIdentity = channelUserIdentity?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedChannelUserIdentity))
        {
            throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
        }

        if (channelType == ChannelTypes.WebsiteMessage && Guid.TryParse(normalizedChannelUserIdentity, out var websiteUserId))
        {
            return websiteUserId;
        }

        UserModel? user = channelType switch
        {
            ChannelTypes.Sms => await _authClient.UserService.GetByPhoneNumberAsync(normalizedChannelUserIdentity),
            ChannelTypes.Email => await _authClient.UserService.GetByEmailAsync(normalizedChannelUserIdentity),
            _ => null
        };

        if (user is not null && user.Id != Guid.Empty)
        {
            return user.Id;
        }

        throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
    }

    private async Task<(Guid userId, string channelUserIdentity)> ResolveTargetUserAsync(
        ChannelTypes channelType,
        Guid? inputUserId,
        string inputChannelUserIdentity)
    {
        if (inputUserId.HasValue && inputUserId.Value != Guid.Empty)
        {
            var users = await _authClient.UserService.GetListByIdsAsync(new[] { inputUserId.Value });
            var user = users.FirstOrDefault(x => x.Id == inputUserId.Value);
            if (user is null)
            {
                throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
            }

            var resolvedChannelUserIdentity = ResolveChannelUserIdentityByUser(channelType, user);
            if (string.IsNullOrWhiteSpace(resolvedChannelUserIdentity))
            {
                throw new UserFriendlyException(errorCode: UnsubscriptionExceptionCodes.CHANNEL_USER_IDENTITY_REQUIRED);
            }

            return (inputUserId.Value, resolvedChannelUserIdentity.Trim());
        }

        var targetUserId = await ResolveTargetUserIdAsync(channelType, inputChannelUserIdentity);
        return (targetUserId, inputChannelUserIdentity.Trim());
    }

    private static string ResolveChannelUserIdentityByUser(ChannelTypes channelType, UserModel user)
    {
        return channelType switch
        {
            ChannelTypes.Sms => user.PhoneNumber ?? string.Empty,
            ChannelTypes.Email => user.Email ?? string.Empty,
            ChannelTypes.WebsiteMessage => user.Id.ToString(),
            _ => string.Empty
        };
    }
}
