// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Channels.Services;

public class ChannelDomainService : DomainService
{
    private readonly IChannelRepository _repository;
    private readonly IServiceProvider _serviceProvider;

    public ChannelDomainService(IDomainEventBus eventBus, IChannelRepository repository, IServiceProvider serviceProvider) : base(eventBus)
    {
        _repository = repository;
        _serviceProvider = serviceProvider;
    }

    public virtual async Task CreateAsync(Channel channel)
    {
        await ValidateChannelAsync(channel.Code);
        await _repository.AddAsync(channel);
        if (channel.Type == ChannelTypes.Sms)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await _repository.UnitOfWork.CommitAsync();
            var channelId = channel.Id;
            Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateAsyncScope();
                var eventBus = scope.ServiceProvider.GetRequiredService<IDomainEventBus>();
                await eventBus.PublishAsync(new SmsTemplateSyncDomainEvent(channelId));
            });
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
            throw new UserFriendlyException("This channel cannot be deleted");
        }
        return await _repository.RemoveAsync(channel);
    }

    protected virtual async Task ValidateChannelAsync(string code, Guid? expectedId = null)
    {
        var dict = await _repository.FindAsync(d => d.Code == code);
        if (dict != null && dict.Id != expectedId)
        {
            throw new UserFriendlyException("Template code cannot be repeated");
        }
    }
}
