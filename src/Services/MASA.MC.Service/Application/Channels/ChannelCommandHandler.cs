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
        await _domainService.CreateAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        if (createCommand.Channel.Type != entity.Type)
            throw new UserFriendlyException("type cannot be changed");
        if (createCommand.Channel.Code != entity.Code)
            throw new UserFriendlyException("code cannot be changed");
        _mapper.Map(createCommand.Channel, entity);
        await _domainService.UpdateAsync(entity);

    }

    [EventHandler]
    public async Task DeleteAsync(DeleteChannelCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        await _domainService.DeleteAsync(entity);

    }
}
