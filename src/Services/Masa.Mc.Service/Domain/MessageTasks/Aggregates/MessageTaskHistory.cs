// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

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

    public MessageTaskHistory(Guid messageTaskId, List<MessageReceiverUser> receiverUsers, bool isTest, DateTimeOffset? sendTime = null)
    {
        MessageTaskId = messageTaskId;
        Status = MessageTaskHistoryStatuses.WaitSend;
        ReceiverUsers = receiverUsers;
        IsTest = isTest;
        SendTime = sendTime;
        TaskHistoryNo = GenerateHistoryNo();
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

        AddDomainEvent(new WithdrawMessageRecordEvent(Id, MessageTaskId));

        if (MessageTask.ChannelType == ChannelType.WebsiteMessage || MessageTask.IsAppInWebsiteMessage)
        {
            AddDomainEvent(new WithdrawWebsiteMessageEvent(Id));
        }

        AddDomainEvent(new UpdateMessageTaskStatusEvent(MessageTaskId));
    }

    public void SetReceiverUsers(List<MessageReceiverUser> receiverUsers)
    {
        ReceiverUsers = receiverUsers;
    }

    public void SetResult(MessageTaskHistoryStatuses status)
    {
        Status = status;
        CompletionTime = DateTimeOffset.Now;

        if (!IsTest)
        {
            AddDomainEvent(new UpdateMessageTaskStatusEvent(MessageTaskId));
        }
    }

    public void SetTaskId(Guid taskId)
    {
        SchedulerTaskId = taskId;
    }

    public void ExecuteTask()
    {
        AddDomainEvent(new ExecuteMessageTaskEvent(MessageTaskId, false));
    }

    public string GenerateHistoryNo()
    {
        return $"SJ{UtilConvert.GetGuidToNumber()}";
    }
}