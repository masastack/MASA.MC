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
        var entity = createCommand.Channel.Adapt<Channel>();
        await _domainService.CreateAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateChannelCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.ChannelId);
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        if (updateCommand.Channel.Type != entity.Type)
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
        if (entity == null)
            throw new UserFriendlyException("channel not found");
        if (await _messageTaskRepository.FindAsync(x => x.ChannelId == createCommand.ChannelId, false) != null)
        {
            throw new UserFriendlyException("There are message tasks in use and cannot be deleted");
        }
        await _domainService.DeleteAsync(entity);
    }
}
