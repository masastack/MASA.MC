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
        if (channel.Type == ChannelType.Sms)
        {
            await _repository.UnitOfWork.SaveChangesAsync();
            await EventBus.PublishAsync(new SmsTemplateSynchroDomainEvent(channel.Id));
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
