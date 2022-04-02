using MASA.MC.Service.Admin.Application.Channels.Commands;
using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Channels.Repositories;
using MASA.MC.Service.Admin.Domain.Channels.Services;

namespace MASA.MC.Service.Admin.Application.Channels;

public class ChannelCommandHandler
{
    private readonly IChannelRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly IMapper _mapper;
    private readonly ChannelDomainService _domainService;

    public ChannelCommandHandler(IChannelRepository repository, IIntegrationEventBus integrationEventBus, IMapper mapper, ChannelDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _mapper = mapper;
        _domainService = domainService;
    }
    [EventHandler]
    public async Task CreateAsync(CreateChannelCommand createCommand)
    {

        var entity = _mapper.Map<Channel>(createCommand.Channel);
        await _repository.AddAsync(entity);
    }
    [EventHandler]
    public async Task UpdateAsync(UpdateChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(createCommand.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        _mapper.Map(createCommand.Channel, entity);
        await _repository.UpdateAsync(entity);

    }
   [EventHandler]
    public async Task DeleteAsync(DeleteChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(createCommand.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        await _domainService.DeleteAsync(entity);

    }
}
