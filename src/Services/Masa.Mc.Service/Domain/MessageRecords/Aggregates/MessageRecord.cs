// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

public class MessageRecord : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; protected set; }
    public Guid ChannelId { get; protected set; }
    public AppChannel Channel { get; protected set; } = default!;
    public Guid MessageTaskId { get; protected set; }
    public AppMessageTask MessageTask { get; protected set; }
    public Guid MessageTaskHistoryId { get; protected set; }
    public bool? Success { get; protected set; }
    public DateTimeOffset? SendTime { get; protected set; }
    public DateTimeOffset? ExpectSendTime { get; protected set; }
    public string FailureReason { get; protected set; } = string.Empty;
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();
    public ExtraPropertyDictionary Variables { get; protected set; } = new();
    public string DisplayName { get; protected set; } = string.Empty;

    public MessageRecord(Guid userId, Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, ExtraPropertyDictionary variables, string displayName, DateTimeOffset? expectSendTime)
    {
        UserId = userId;
        ChannelId = channelId;
        MessageTaskId = messageTaskId;
        MessageTaskHistoryId = messageTaskHistoryId;
        Variables = variables;
        DisplayName = displayName;
        ExpectSendTime = expectSendTime;
    }

    public void SetResult(bool success, string failureReason)
    {
        SendTime = DateTimeOffset.Now;
        Success = success;
        FailureReason = failureReason;
    }

    public virtual T GetDataValue<T>(string name)
    {
        return ExtraProperties.GetProperty<T>(name);
    }

    public virtual void SetDataValue(string name, string value)
    {
        ExtraProperties.SetProperty(name, value);
    }

    public void SetWithdraw()
    {
        Success = false;
        FailureReason = "Recall message";
    }
}
