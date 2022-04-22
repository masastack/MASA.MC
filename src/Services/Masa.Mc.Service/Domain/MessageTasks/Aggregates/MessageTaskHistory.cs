namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskHistory : AuditAggregateRoot<Guid, Guid>
{
    public Guid MessageTaskId { get; set; }

    public MessageTaskHistoryStatus Status { get; set; }

    public ExtraPropertyDictionary Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendingRules { get; protected set; } = new();
}