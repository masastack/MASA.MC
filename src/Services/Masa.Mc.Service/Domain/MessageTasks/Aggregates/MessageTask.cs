namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTask : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public string DisplayName { get; protected set; } = string.Empty;

    public Guid ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityType EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public bool IsDraft { get; protected set; }

    public bool IsEnabled { get; protected set; }

    public ReceiverType ReceiverType { get; protected set; }

    public DateTime? SendTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendingRules { get; protected set; } = new();

    public ICollection<MessageTaskHistory> Historys { get; protected set; }

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public bool IsDeleted { get; protected set; }

    public MessageTask(string displayName, Guid channelId, MessageEntityType entityType, Guid entityId, bool isDraft, string sign, ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules)
    {
        DisplayName = displayName;
        ChannelId = channelId;
        EntityType = entityType;
        EntityId = entityId;
        Sign = sign;
        SetDraft(isDraft);
        SetReceivers(receiverType, receivers);
        SendingRules = sendingRules ?? new();
        Historys = new Collection<MessageTaskHistory>();
    }

    public virtual void SendTask(ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        SetDraft(false);
        SetReceivers(receiverType, receivers);
        SendingRules = sendingRules ?? new();
        SendTime = sendTime ?? DateTime.UtcNow;
        Sign = sign;
        Variables = variables;
        AddHistory(ReceiverType, Receivers, SendingRules, SendTime, Sign, Variables);
    }

    public virtual void SetEnabled()
    {
        IsEnabled = true;
    }

    public virtual void SetDisable()
    {
        IsEnabled = false;
    }

    public virtual void AddHistory(ReceiverType receiverType, ExtraPropertyDictionary receivers, ExtraPropertyDictionary sendingRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        Historys.Add(new MessageTaskHistory(Id, receiverType, receivers, sendingRules, sendTime, sign, variables));
    }

    public virtual void SetReceivers(ReceiverType receiverType, ExtraPropertyDictionary receivers)
    {
        ReceiverType = receiverType;
        if (receiverType == ReceiverType.Assign)
        {
            Receivers = receivers;
        }
    }

    public virtual void SetEntity(MessageEntityType entityType, Guid entityId, string displayName)
    {
        EntityType = entityType;
        EntityId = entityId;
        DisplayName = displayName;
    }

    public virtual void SetDraft(bool isDraft)
    {
        IsDraft = isDraft;
        if (isDraft)
        {
            SetDisable();
        }
        else
        {
            SetEnabled();
        }
    }

    public virtual void UpdateVariables(ExtraPropertyDictionary variables)
    {
        Variables = variables;
    }
}