namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTask : AuditAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityType EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public bool IsEnabled { get; protected set; }

    public ReceiverType ReceiverType { get; protected set; }

    public DateTime? SendTime { get; protected set; }

    public ExtraPropertyDictionary Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendingRules { get; protected set; } = new();

    public ICollection<MessageTaskHistory> Historys { get; protected set; }

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public MessageTask(Guid channelId, MessageEntityType entityType, Guid entityId, bool isEnabled, ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules)
    {
        ChannelId = channelId;
        EntityType = entityType;
        EntityId = entityId;
        if (isEnabled)
        {
            SetEnabled();
        }
        else
        {
            SetDisable();
        }
        SetReceivers(receiverType, receivers);
        SendingRules = sendingRules ?? new();
        Historys = new Collection<MessageTaskHistory>();
    }

    public virtual void SetEnabled()
    {
        IsEnabled = true;
    }

    public virtual void SetDisable()
    {
        IsEnabled = false;
    }

    public virtual void AddHistory(ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules)
    {
        Historys.Add(new MessageTaskHistory(Id, receiverType, receivers, sendingRules));
        SendTime = DateTime.UtcNow;
    }

    public virtual void SetReceivers(ReceiverType receiverType, ExtraPropertyDictionary receivers)
    {
        ReceiverType = receiverType;
        if (receiverType == ReceiverType.Assign)
        {
            Receivers = receivers;
        }
    }

    public virtual void SetEntity(MessageEntityType entityType, Guid entityId)
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}