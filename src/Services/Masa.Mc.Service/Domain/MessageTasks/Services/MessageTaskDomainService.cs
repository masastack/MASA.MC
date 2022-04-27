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
        if (!messageTask.IsDraft)
        {
            messageTask.SetSendTime();
            messageTask.SetEnabled();
            messageTask.AddHistory(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendingRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDisable();
        }
        await _repository.AddAsync(messageTask);
    }

    public virtual async Task UpdateAsync(MessageTask messageTask)
    {
        if (!messageTask.IsDraft)
        {
            messageTask.SetSendTime();
            messageTask.SetEnabled();
            messageTask.AddHistory(messageTask.ReceiverType, messageTask.Receivers, messageTask.SendingRules, messageTask.SendTime, messageTask.Sign, messageTask.Variables);
        }
        else
        {
            messageTask.SetDisable();
        }
        await _repository.UpdateAsync(messageTask);
    }

    public virtual async Task SendAsync(Guid messageTaskId, ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        var messageTask = await _repository.FindAsync(x => x.Id == messageTaskId);
        if (messageTask == null)
            throw new UserFriendlyException("messageTask not found");
        messageTask.AddHistory(receiverType, receivers, sendingRules, sendTime, sign, variables);
        await _repository.UpdateAsync(messageTask);
    }
}
