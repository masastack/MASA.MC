// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

/// <summary>
/// Subsequently, it is intended to be a single aggregation root
/// </summary>
public class MessageTaskHistory : FullAggregateRoot<Guid, Guid>
{
    public Guid MessageTaskId { get; protected set; }

    public string TaskHistoryNo { get; protected set; } = string.Empty;

    public MessageTask MessageTask { get; protected set; }

    public ReceiverTypes ReceiverType { get; protected set; }

    public MessageTaskReceiverSelectTypes ReceiverSelectType { get; protected set; }

    public MessageTaskHistoryStatuses Status { get; protected set; }

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendRules { get; protected set; } = new();

    public DateTime? SendTime { get; protected set; }

    public DateTime? CompletionTime { get; protected set; }

    public DateTime? WithdrawTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public ICollection<MessageReceiverUser> ReceiverUsers { get; protected set; } = new Collection<MessageReceiverUser>();

    protected internal MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, ReceiverTypes receiverType, MessageTaskReceiverSelectTypes receiverSelectType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        MessageTaskId = messageTaskId;
        TaskHistoryNo = taskHistoryNo;
        ReceiverType = receiverType;
        ReceiverSelectType = receiverSelectType;
        Receivers = receivers;
        SendRules = sendRules;
        SendTime = sendTime;
        Status = MessageTaskHistoryStatuses.WaitSend;
        Sign = sign;
        Variables = variables;
    }

    public void SetSending()
    {
        SendTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatuses.Sending;
    }

    public void SetComplete()
    {
        CompletionTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatuses.Completed;
    }

    public void SetWithdraw()
    {
        WithdrawTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatuses.Withdrawn;
    }

    public void SetReceiverUsers(ICollection<MessageReceiverUser> receiverUsers)
    {
        ReceiverUsers = receiverUsers;
    }
}