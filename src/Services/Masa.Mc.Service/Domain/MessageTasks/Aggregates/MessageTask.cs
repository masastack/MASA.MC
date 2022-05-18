// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTask : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public string DisplayName { get; protected set; } = string.Empty;

    public Guid ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityTypes EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public bool IsDraft { get; protected set; }

    public bool IsEnabled { get; protected set; }

    public ReceiverTypes ReceiverType { get; protected set; }

    public MessageTaskReceiverSelectTypes ReceiverSelectType { get; protected set; }

    public DateTime? SendTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendRules { get; protected set; } = new();

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public bool IsDeleted { get; protected set; }

    public MessageTask(string displayName, Guid channelId, MessageEntityTypes entityType, Guid entityId, bool isDraft, string sign, ReceiverTypes receiverType, MessageTaskReceiverSelectTypes receiverSelectType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules)
    {
        DisplayName = displayName;
        ChannelId = channelId;
        EntityType = entityType;
        EntityId = entityId;
        Sign = sign;
        SetDraft(isDraft);
        ReceiverSelectType = receiverSelectType;
        SetReceivers(receiverType, receivers);
        SendRules = sendRules ?? new();
    }

    public virtual void SendTask(ReceiverTypes receiverType, List<MessageTaskReceiver> receivers, MessageTaskReceiverSelectTypes receiverSelectType, ExtraPropertyDictionary sendRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        SetDraft(false);
        ReceiverSelectType = receiverSelectType;
        SetReceivers(receiverType, receivers);
        SendRules = sendRules ?? new();
        SendTime = sendTime ?? DateTime.UtcNow;
        Sign = sign;
        Variables = variables;
    }

    public virtual void SetEnabled()
    {
        IsEnabled = true;
    }

    public virtual void SetDisable()
    {
        IsEnabled = false;
    }

    public virtual void SetReceivers(ReceiverTypes receiverType, List<MessageTaskReceiver> receivers)
    {
        ReceiverType = receiverType;
        if (receiverType == ReceiverTypes.Assign)
        {
            Receivers = receivers;
        }
    }

    public virtual void SetEntity(MessageEntityTypes entityType, Guid entityId, string displayName)
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