﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageRecords.Aggregates;

public class MessageRecord : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; protected set; }
    public Guid ChannelId { get; protected set; }
    public AppChannel Channel { get; protected set; } = default!;
    public Guid MessageTaskId { get; protected set; }
    public Guid MessageTaskHistoryId { get; protected set; }
    public bool? Success { get; protected set; }
    public DateTimeOffset? SendTime { get; protected set; }
    public DateTimeOffset? ExpectSendTime { get; protected set; }
    public string FailureReason { get; protected set; } = string.Empty;
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();
    public ExtraPropertyDictionary Variables { get; protected set; } = new();
    public string DisplayName { get; protected set; } = string.Empty;
    public MessageEntityTypes MessageEntityType { get; protected set; }
    public Guid MessageEntityId { get; protected set; }
    public string ChannelUserIdentity { get; protected set; } = string.Empty;
    public string SystemId { get; protected set; } = string.Empty;

    public MessageRecord(Guid userId, string channelUserIdentity, Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, ExtraPropertyDictionary variables, string displayName, DateTimeOffset? expectSendTime, string systemId)
    {
        UserId = userId;
        ChannelUserIdentity = channelUserIdentity;
        ChannelId = channelId;
        MessageTaskId = messageTaskId;
        MessageTaskHistoryId = messageTaskHistoryId;
        Variables = variables;
        DisplayName = displayName;
        ExpectSendTime = expectSendTime;
        SystemId = systemId;
    }

    public void SetResult(bool success, string failureReason, DateTimeOffset? sendTime = null)
    {
        SendTime = sendTime ?? DateTimeOffset.UtcNow;
        Success = success;
        FailureReason = failureReason;

        if (UserId == default && Id == default)
        {
            Id = IdGeneratorFactory.SequentialGuidGenerator.NewId();
            AddDomainEvent(new UpdateMessageRecordUserEvent(Id));
        }
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

    public void SetMessageEntity(MessageEntityTypes messageEntityType, Guid messageEntityId)
    {
        MessageEntityType = messageEntityType;
        MessageEntityId = messageEntityId;
    }

    public void SetUserId(Guid userId)
    {
        UserId = userId;
    }

    public void SetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}
