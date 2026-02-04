// Copyright (c) MASA Stack All rights reserved.
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

    public string MessageId { get; protected set; } = string.Empty;

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

    public void SetResult(bool? success, string message, DateTimeOffset? sendTime = null, string messageId = "")
    {
        SendTime = sendTime ?? DateTimeOffset.UtcNow;
        Success = success;
        if (success == false)
        {
            FailureReason = message;
        }
        MessageId = messageId;

        if (UserId == default && Id == default)
        {
            Id = IdGeneratorFactory.SequentialGuidGenerator.NewId();
            AddDomainEvent(new UpdateMessageRecordUserEvent(Id));
        }

        if (Success == false && IsCompensate && UserId != Guid.Empty)
        {
            AddCompensateDomainEvent();
        }
    }

    private void AddCompensateDomainEvent()
    {
        var compensateChannelCode = ExtraProperties.GetProperty<string>(CompensateConsts.CHANNEL_CODE) ?? string.Empty;
        var compensateTemplateCode = ExtraProperties.GetProperty<string>(CompensateConsts.TEMPLATE_CODE) ?? string.Empty;
        var compensateVariablesStr = ExtraProperties.GetProperty<string>(CompensateConsts.VARIABLES) ?? string.Empty;
        var compensateVariablesDict = QueryHelpers.ParseQuery(compensateVariablesStr)
            .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.ToString());
        var compensateVariables = new ExtraPropertyDictionary(compensateVariablesDict);

        AddDomainEvent(new CompensateMessageEvent(UserId, compensateChannelCode, compensateTemplateCode, compensateVariables));
    }

    public void UpdateResult(bool success, string failureReason)
    {
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

    public void SetCompensate(ExtraPropertyDictionary extraProperties)
    {
        SetDataValue(CompensateConsts.CHANNEL_CODE, extraProperties.GetProperty<string>(CompensateConsts.CHANNEL_CODE) ?? string.Empty);
        SetDataValue(CompensateConsts.TEMPLATE_CODE, extraProperties.GetProperty<string>(CompensateConsts.TEMPLATE_CODE) ?? string.Empty);
        SetDataValue(CompensateConsts.VARIABLES, extraProperties.GetProperty<string>(CompensateConsts.VARIABLES) ?? string.Empty);
    }

    public bool IsCompensate
    {
        get
        {
            if (ExtraProperties == null) return false;

            return !string.IsNullOrEmpty(ExtraProperties.GetProperty<string>(CompensateConsts.CHANNEL_CODE)) &&
                   !string.IsNullOrEmpty(ExtraProperties.GetProperty<string>(CompensateConsts.TEMPLATE_CODE));
        }
    }
}
