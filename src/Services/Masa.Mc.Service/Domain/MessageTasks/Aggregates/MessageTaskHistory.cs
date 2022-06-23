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

    public MessageTaskHistoryStatuses Status { get; protected set; }

    public DateTimeOffset? SendTime { get; protected set; }

    public DateTimeOffset? CompletionTime { get; protected set; }

    public DateTimeOffset? WithdrawTime { get; protected set; }

    public List<MessageReceiverUser> ReceiverUsers { get; protected set; } = new();

    public MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, List<MessageReceiverUser> receiverUsers)
    {
        MessageTaskId = messageTaskId;
        TaskHistoryNo = taskHistoryNo;
        Status = MessageTaskHistoryStatuses.WaitSend;
        ReceiverUsers = receiverUsers;
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

    public void SetReceiverUsers(List<MessageReceiverUser> receiverUsers)
    {
        ReceiverUsers = receiverUsers;
    }

    public void SetResult(MessageTaskHistoryStatuses status)
    {
        Status = status;
        CompletionTime = DateTimeOffset.Now;
    }
}