// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Channels.Services;

public class ChannelDomainService : DomainService
{
    private readonly IChannelRepository _repository;

    private readonly IUnitOfWork _unitOfWork;

    public ChannelDomainService(IDomainEventBus eventBus, IChannelRepository repository, IUnitOfWork unitOfWork) : base(eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task CreateAsync(Channel channel)
    {
        await ValidateChannelAsync(channel.Code);
        await _repository.AddAsync(channel);
        if (channel.Type.Id == ChannelType.Sms.Id)
        {
            await _unitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new SmsTemplateSynchroIntegrationDomainEvent(channel.Id));
        }
    }

    public virtual async Task UpdateAsync(Channel channel)
    {
        await ValidateChannelAsync(channel.Code, channel.Id);
        await _repository.UpdateAsync(channel);
    }

    public virtual async Task<Channel> DeleteAsync(Channel channel)
    {
        if (channel.IsStatic)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_CANNOT_DELETED);
        }
        return await _repository.RemoveAsync(channel);
    }

    protected virtual async Task ValidateChannelAsync(string code, Guid? expectedId = null)
    {
        var dict = await _repository.FindAsync(d => d.Code == code);
        if (dict != null && dict.Id != expectedId)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_CANNOT_REPEATED);
        }
    }
}
