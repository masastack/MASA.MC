namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Services;

public class MessageTaskDomainService : DomainService
{
    private readonly IMessageTaskRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageTaskDomainService(IDomainEventBus eventBus, IMessageTaskRepository repository, IUnitOfWork unitOfWork) : base(eventBus)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public virtual async Task CreateAsync(MessageTask messageTask)
    {
        if (messageTask.IsEnabled)
        {
            messageTask.AddHistory(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendingRules);
        }
        await _repository.AddAsync(messageTask);
    }

    public virtual async Task ExecuteAsync(Guid messageTaskId, ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules)
    {
        var messageTask = await _repository.FindAsync(x=>x.Id== messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        messageTask.AddHistory(receiverType, receivers, sendingRules);
    }
}
