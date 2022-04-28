namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskSendingRule
{
    public long Timeout { get; protected set; }
    public long RetryInterval { get; protected set; }
    public int RetryCount { get; protected set; }
    public bool IsSendingInterval { get; protected set; }
    public long SendingInterval { get; protected set; }
    public long SendingCount { get; protected set; }
}
