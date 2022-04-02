using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Channels.Repositories;

namespace MASA.MC.Service.Admin.Domain.Channels.Services;

public class ChannelDomainService : DomainService
{
    private readonly IChannelRepository _channelRepository;

    public ChannelDomainService(IDomainEventBus eventBus, IChannelRepository channelRepository) : base(eventBus)
    {
        _channelRepository = channelRepository;
    }

    public async Task<Channel> DeleteAsync(Channel channel)
    {
        if (channel.IsStatic)
        {
            throw new UserFriendlyException("该渠道不允许删除");
        }

        return await _channelRepository.RemoveAsync(channel);
    }
}
