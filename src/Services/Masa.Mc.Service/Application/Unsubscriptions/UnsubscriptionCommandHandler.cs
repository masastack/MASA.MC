// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions;

public class UnsubscriptionCommandHandler
{
    private readonly UnsubscriptionDomainService _unsubscriptionDomainService;
    private readonly IUserContext _userContext;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public UnsubscriptionCommandHandler(
        UnsubscriptionDomainService unsubscriptionDomainService,
        IUserContext userContext,
        IChannelRepository channelRepository,
        IMessageTemplateRepository messageTemplateRepository)
    {
        _unsubscriptionDomainService = unsubscriptionDomainService;
        _userContext = userContext;
        _channelRepository = channelRepository;
        _messageTemplateRepository = messageTemplateRepository;
    }

    [EventHandler]
    public async Task AddChannelUserIdentityToBlacklistAsync(AddChannelUserIdentityToUnsubscriptionBlacklistCommand command)
    {
        var channel = await ResolveChannelAsync(command.Input.ChannelCode);
        var templateId = await ResolveTemplateIdAsync(channel.Id, command.Input.TemplateCode);

        var channelType = (ChannelTypes)Enum.Parse(typeof(ChannelTypes), channel.Type.Id.ToString());
        await _unsubscriptionDomainService.AddChannelUserIdentityToBlacklistAsync(
            _userContext.GetUserId<Guid>(),
            channel.Id,
            channelType,
            channel.Provider,
            command.Input.ChannelUserIdentity,
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
}
