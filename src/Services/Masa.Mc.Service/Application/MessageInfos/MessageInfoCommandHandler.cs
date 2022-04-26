namespace Masa.Mc.Service.Admin.Application.MessageInfos;

public class MessageInfoCommandHandler
{
    private readonly IMessageInfoRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;

    public MessageInfoCommandHandler(IMessageInfoRepository repository, IIntegrationEventBus integrationEventBus)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
    }

    [EventHandler]
    public async Task CreateAsync(CreateMessageInfoCommand createCommand)
    {
        var entity = createCommand.MessageInfo.Adapt<MessageInfo>();
        await _repository.AddAsync(entity);
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateMessageInfoCommand updateCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == updateCommand.MessageInfoId);
        if (entity == null)
            throw new UserFriendlyException("messageInfo not found");
        updateCommand.MessageInfo.Adapt(entity);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageInfoCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageInfoId);
        if (entity == null)
            throw new UserFriendlyException("messageInfo not found");
        await _repository.RemoveAsync(entity);
    }
}
