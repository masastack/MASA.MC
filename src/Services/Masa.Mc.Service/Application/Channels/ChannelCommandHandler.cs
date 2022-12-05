// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels;

public class ChannelCommandHandler
{
    private readonly IChannelRepository _repository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly ChannelDomainService _domainService;

    public ChannelCommandHandler(IChannelRepository repository, IMessageTaskRepository messageTaskRepository, ChannelDomainService domainService)
    {
        _repository = repository;
        _messageTaskRepository = messageTaskRepository;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task CreateAsync(CreateChannelCommand createCommand)
    {
        await ValidateChannelNameAsync(createCommand.Channel.DisplayName, null);
        var entity = createCommand.Channel.Adapt<Channel>();
        await _domainService.CreateAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateChannelCommand updateCommand)
    {
        await ValidateChannelNameAsync(updateCommand.Channel.DisplayName, updateCommand.ChannelId);
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.ChannelId);
        MasaArgumentException.ThrowIfNull(entity, "Channel");
        if ((int)updateCommand.Channel.Type != entity.Type.Id)
            throw new UserFriendlyException("type cannot be changed");
        if (updateCommand.Channel.Code != entity.Code)
            throw new UserFriendlyException("code cannot be changed");
        updateCommand.Channel.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ChannelId);
        MasaArgumentException.ThrowIfNull(entity, "Channel");
        if (await _messageTaskRepository.FindAsync(x => x.ChannelId == createCommand.ChannelId && (x.Status == MessageTaskStatuses.WaitSend || x.Status == MessageTaskStatuses.Sending), false) != null)
        {
            throw new UserFriendlyException("If there is a message task to be sent / being sent, it cannot be deleted");
        }
        await _domainService.DeleteAsync(entity);
    }

    private async Task ValidateChannelNameAsync(string displayName, Guid? id)
    {
        if (await _repository.FindAsync(x => x.DisplayName == displayName && x.Id != id) != null)
        {
            throw new UserFriendlyException("channel name cannot be repeated");
        }
    }
}
