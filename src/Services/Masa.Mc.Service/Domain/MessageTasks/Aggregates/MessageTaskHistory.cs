// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageTaskHistory : AuditEntity<Guid, Guid>
{
    public Guid MessageTaskId { get; protected set; }

    public ReceiverTypes ReceiverType { get; protected set; }

    public MessageTaskHistoryStatues Status { get; protected set; }

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendRules { get; protected set; } = new();

    public DateTime? SendTime { get; protected set; }

    public DateTime? CompletionTime { get; protected set; }

    public DateTime? WithdrawTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    protected internal MessageTaskHistory(Guid messageTaskId, ReceiverTypes receiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTime? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        MessageTaskId = messageTaskId;
        ReceiverType = receiverType;
        Receivers = receivers;
        SendRules = sendRules;
        SendTime = sendTime;
        Status = MessageTaskHistoryStatues.WaitSend;
        Sign = sign;
        Variables = variables;
    }

    public void SetSend()
    {
        SendTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatues.Sending;
    }

    public void SetComplete()
    {
        CompletionTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatues.Completed;
    }

    public void SetWithdraw()
    {
        WithdrawTime = DateTime.UtcNow;
        Status = MessageTaskHistoryStatues.Withdrawn;
    }
}