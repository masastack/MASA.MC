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

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; protected set; }

    public MessageTaskHistoryStatuses Status { get; protected set; }

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public ExtraPropertyDictionary SendRules { get; protected set; } = new();

    public DateTimeOffset? SendTime { get; protected set; }

    public DateTimeOffset? CompletionTime { get; protected set; }

    public DateTimeOffset? WithdrawTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public ICollection<MessageReceiverUser> ReceiverUsers { get; protected set; } = new Collection<MessageReceiverUser>();

    protected internal MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, ReceiverTypes receiverType, MessageTaskSelectReceiverTypes selectReceiverType, List<MessageTaskReceiver> receivers, ExtraPropertyDictionary sendRules, DateTimeOffset? sendTime, string sign, ExtraPropertyDictionary variables)
    {
        MessageTaskId = messageTaskId;
        TaskHistoryNo = taskHistoryNo;
        ReceiverType = receiverType;
        SelectReceiverType = selectReceiverType;
        Receivers = receivers;
        SendRules = sendRules;
        SendTime = sendTime;
        Status = MessageTaskHistoryStatuses.WaitSend;
        Sign = sign;
        Variables = variables;
    }

    public void SetSending()
    {
        SendTime = DateTimeOffset.Now;
        Status = MessageTaskHistoryStatuses.Sending;
    }

    public void SetWithdraw()
    {
        WithdrawTime = DateTimeOffset.Now;
        Status = MessageTaskHistoryStatuses.Withdrawn;
    }

    public void SetReceiverUsers(ICollection<MessageReceiverUser> receiverUsers)
    {
        ReceiverUsers = receiverUsers;
    }

    public void SetResult(MessageTaskHistoryStatuses status)
    {
        Status = status;
        CompletionTime = DateTimeOffset.Now;
    }
}