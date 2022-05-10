// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

public class MessageRecord : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public Guid UserId { get; protected set; }
    public Guid ChannelId { get; protected set; }
    public AppChannel Channel { get; protected set; } = default!;
    public Guid MessageTaskId { get; protected set; }
    public Guid MessageTaskHistoryId { get; protected set; }
    public bool? Success { get; protected set; }
    public DateTime? SendTime { get; protected set; }
    public string FailureReason { get; protected set; } = string.Empty;
    public bool IsDeleted { get; protected set; }
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public MessageRecord(Guid userId, Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, ExtraPropertyDictionary variables)
    {
        UserId = userId;
        ChannelId = channelId;
        MessageTaskId = messageTaskId;
        MessageTaskHistoryId = messageTaskHistoryId;
        Variables = variables;
    }

    public void SetResult(bool success, string failureReason)
    {
        SendTime = DateTime.UtcNow;
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
}
