// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTask : FullAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public ChannelTypes? ChannelType { get; protected set; }

    public Guid? ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityTypes EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public bool IsDraft { get; protected set; }

    public bool IsEnabled { get; protected set; }

    public ReceiverTypes ReceiverType { get; protected set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; protected set; }

    public DateTimeOffset? SendTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public List<MessageReceiverUser> ReceiverUsers { get; protected set; } = new();

    public MessageTaskSendingRule SendRules { get; protected set; } = new();

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public MessageTaskStatuses Status { get; protected set; }

    public MessageTask(string displayName, ChannelTypes? channelType, Guid? channelId, MessageEntityTypes entityType, Guid entityId, bool isDraft, string sign, ReceiverTypes receiverType, MessageTaskSelectReceiverTypes selectReceiverType, List<MessageTaskReceiver> receivers, MessageTaskSendingRule sendRules)
    {
        DisplayName = displayName;
        ChannelType = channelType;
        ChannelId = channelId;
        EntityType = entityType;
        EntityId = entityId;
        Sign = sign;
        SetDraft(isDraft);
        SelectReceiverType = selectReceiverType;
        SetReceivers(receiverType, receivers);
        SendRules = sendRules ?? new();
        Status = MessageTaskStatuses.WaitSend;
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

    public void SetSending()
    {
        SendTime = DateTimeOffset.Now;
        Status = MessageTaskStatuses.Sending;
    }

    public void SetResult(MessageTaskStatuses status)
    {
        Status = status;
    }
}