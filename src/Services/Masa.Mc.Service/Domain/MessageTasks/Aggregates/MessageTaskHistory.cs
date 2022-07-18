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

    public MessageTask MessageTask { get; protected set; } = default!;

    public MessageTaskHistoryStatuses Status { get; protected set; }

    public DateTimeOffset? SendTime { get; protected set; }

    public DateTimeOffset? CompletionTime { get; protected set; }

    public DateTimeOffset? WithdrawTime { get; protected set; }

    public bool IsTest { get; protected set; }

    public List<MessageReceiverUser> ReceiverUsers { get; protected set; } = new();

    public Guid SchedulerTaskId { get; protected set; }

    public MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, List<MessageReceiverUser> receiverUsers, bool isTest, DateTimeOffset? sendTime = null)
    {
        MessageTaskId = messageTaskId;
        TaskHistoryNo = taskHistoryNo;
        Status = MessageTaskHistoryStatuses.WaitSend;
        ReceiverUsers = receiverUsers;
        IsTest = isTest;
        SendTime = sendTime;
    }

    public MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, MessageTaskHistoryStatuses status, DateTimeOffset? sendTime, DateTimeOffset? completionTime, DateTimeOffset? withdrawTime) : this(messageTaskId, taskHistoryNo, status, sendTime, completionTime, withdrawTime, new List<MessageReceiverUser>())
    {

    }

    public MessageTaskHistory(Guid messageTaskId, string taskHistoryNo, MessageTaskHistoryStatuses status, DateTimeOffset? sendTime, DateTimeOffset? completionTime, DateTimeOffset? withdrawTime, List<MessageReceiverUser> receiverUsers)
    {
        MessageTaskId = messageTaskId;
        TaskHistoryNo = taskHistoryNo;
        Status = status;
        SendTime = sendTime;
        CompletionTime = completionTime;
        WithdrawTime = withdrawTime;
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
        AddDomainEvent(new UpdateMessageTaskStatusEvent(MessageTaskId));
    }

    public void SetTaskId(Guid taskId)
    {
        SchedulerTaskId = taskId;
    }
}