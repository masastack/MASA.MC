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
}
