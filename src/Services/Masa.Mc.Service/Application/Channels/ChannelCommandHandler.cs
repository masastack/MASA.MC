// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Channels;

public class ChannelCommandHandler
{
    private readonly IChannelRepository _repository;
    private readonly IAppVendorConfigRepository _appVendorConfigRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly ChannelDomainService _domainService;
    private readonly II18n<DefaultResource> _i18n;

    public ChannelCommandHandler(IChannelRepository repository, IAppVendorConfigRepository appVendorConfigRepository, IMessageTaskRepository messageTaskRepository, ChannelDomainService domainService, II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _appVendorConfigRepository = appVendorConfigRepository;
        _messageTaskRepository = messageTaskRepository;
        _domainService = domainService;
        _i18n = i18n;
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
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("Channel"));
        if ((int)updateCommand.Channel.Type != entity.Type.Id)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_TYPE_CANNOT_BE_MODIFIED);
        if (updateCommand.Channel.Code != entity.Code)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_CODE_CANNOT_BE_MODIFIED);
        updateCommand.Channel.Adapt(entity);
        await _domainService.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ChannelId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("Channel"));
        if (await _messageTaskRepository.FindAsync(x => x.ChannelId == createCommand.ChannelId && !x.IsDraft && (x.Status == MessageTaskStatuses.WaitSend || x.Status == MessageTaskStatuses.Sending), false) != null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TASK_TO_BE_SENT_OR_BEING_SENT_CANNOT_BE_DELETED);
        }
        await _domainService.DeleteAsync(entity);
    }


    [EventHandler]
    public async Task SaveVendorConfigAsync(SaveChannelVendorConfigCommand command)
    {
        var entity = await _appVendorConfigRepository.FindAsync(x => x.ChannelId == command.ChannelId && x.Vendor == command.Vendor);

        if (entity is null)
        {
            entity = new AppVendorConfig(command.ChannelId, command.Vendor, command.Options);
            await _appVendorConfigRepository.AddAsync(entity);
        }
        else
        {
            entity.UpdateOptions(command.Options);
            await _appVendorConfigRepository.UpdateAsync(entity);
        }
    }

    [EventHandler]
    public async Task SaveVendorsAsync(SaveChannelVendorsCommand command)
    {
        var existingConfigs = await _appVendorConfigRepository.GetListAsync(x => x.ChannelId == command.ChannelId);
        var existingVendors = existingConfigs.ToDictionary(x => x.Vendor, x => x);
        var newVendors = command.Vendors.ToDictionary(x => x.Vendor, x => x);

        // Update existing and remove deleted vendors
        foreach (var existingConfig in existingConfigs)
        {
            if (newVendors.TryGetValue(existingConfig.Vendor, out var newConfig))
            {
                existingConfig.UpdateOptions(newConfig.Options);
                await _appVendorConfigRepository.UpdateAsync(existingConfig);
            }
            else
            {
                await _appVendorConfigRepository.RemoveAsync(existingConfig);
            }
        }

        // Add new vendors
        foreach (var vendorConfig in command.Vendors.Where(v => !existingVendors.ContainsKey(v.Vendor)))
        {
            var entity = new AppVendorConfig(command.ChannelId, vendorConfig.Vendor, vendorConfig.Options);
            await _appVendorConfigRepository.AddAsync(entity);
        }
    }

    private async Task ValidateChannelNameAsync(string displayName, Guid? id)
    {
        if (await _repository.FindAsync(x => x.DisplayName == displayName && x.Id != id) != null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_NAME_CANNOT_BE_DUPLICATE);
        }
    }
}
