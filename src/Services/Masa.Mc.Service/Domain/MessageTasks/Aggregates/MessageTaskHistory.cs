namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskHistory : AuditEntity<Guid, Guid>
{
    public Guid MessageTaskId { get; protected set; }

    public ReceiverType ReceiverType { get; protected set; }

    public MessageTaskHistoryStatus Status { get; protected set; }

    public ExtraPropertyDictionary Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendingRules { get; protected set; } = new();

    public DateTime? SendTime { get; protected set; }

    public DateTime? CompletionTime { get; protected set; }

    public DateTime? WithdrawTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    protected internal MessageTaskHistory(Guid messageTaskId, ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        MessageTaskId = messageTaskId;
        ReceiverType = receiverType;
        Receivers = receivers;
        SendingRules = sendingRules;
        SendTime = sendTime;
        Status = MessageTaskHistoryStatus.WaitSend;
        Sign = sign;
        Variables = variables;
    }

    public void SetSend()
    {
        SendTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatus.Sending;
    }

    public void SetComplete()
    {
        CompletionTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatus.Completed;
    }

    public void SetWithdraw()
    {
        WithdrawTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatus.Withdrawn;
    }
}