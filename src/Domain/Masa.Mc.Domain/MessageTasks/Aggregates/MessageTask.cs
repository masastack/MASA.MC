// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Aggregates;

public class MessageTask : FullAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public ChannelType? ChannelType { get; protected set; }

    public Guid? ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public MessageEntityTypes EntityType { get; protected set; }

    public Guid EntityId { get; protected set; }

    public bool IsDraft { get; protected set; }

    public bool IsEnabled { get; protected set; }

    public ReceiverTypes ReceiverType { get; protected set; }

    public MessageTaskSelectReceiverTypes SelectReceiverType { get; protected set; }

    public DateTimeOffset? SendTime { get; protected set; }

    public DateTimeOffset? ExpectSendTime { get; protected set; }

    public string Sign { get; protected set; } = string.Empty;

    public List<MessageTaskReceiver> Receivers { get; protected set; } = new();

    public MessageTaskSendingRule SendRules { get; protected set; } = default!;

    public ExtraPropertyDictionary Variables { get; protected set; } = new();

    public MessageTaskStatuses Status { get; protected set; }

    public MessageTaskSources Source { get; protected set; }

    public Guid SchedulerJobId { get; protected set; }

    public string SystemId { get; protected set; } = string.Empty;

    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    private MessageTask() { }

    public MessageTask(string displayName, ChannelType? channelType, Guid? channelId, MessageEntityTypes entityType, Guid entityId, bool isDraft, string sign, ReceiverTypes receiverType, MessageTaskSelectReceiverTypes selectReceiverType, List<MessageTaskReceiver> receivers, MessageTaskSendingRule sendRules, MessageTaskSources source, ExtraPropertyDictionary extraProperties)
    {
        DisplayName = displayName;
        ChannelType = channelType;
        ChannelId = channelId;
        EntityType = entityType;
        EntityId = entityId;
        Sign = sign;
        SetDraft(isDraft);
        SelectReceiverType = selectReceiverType;
        SetReceivers(receiverType, receivers);
        SendRules = sendRules;
        Status = MessageTaskStatuses.WaitSend;
        Source = source;
        ExtraProperties = extraProperties;
    }

    public virtual void SetEnabled()
    {
        if (!IsDraft)
        {
            IsEnabled = true;
        }
    }

    public virtual void SetDisable()
    {
        IsEnabled = false;
    }

    public virtual void SetReceivers(ReceiverTypes receiverType, List<MessageTaskReceiver> receivers)
    {
        ReceiverType = receiverType;
        if (receiverType == ReceiverTypes.Assign)
        {
            Receivers = receivers;
        }
    }

    public virtual void SetEntity(MessageEntityTypes entityType, Guid entityId, string displayName)
    {
        EntityType = entityType;
        EntityId = entityId;
        DisplayName = displayName;
    }

    public virtual void SetDraft(bool isDraft)
    {
        IsDraft = isDraft;
        if (isDraft)
        {
            SetDisable();
        }
        else
        {
            SetEnabled();
        }
    }

    public virtual void UpdateVariables(ExtraPropertyDictionary variables)
    {
        Variables = variables;
    }

    public void SetSending()
    {
        SendTime = DateTimeOffset.UtcNow;
        Status = MessageTaskStatuses.Sending;
    }

    public void SetResult(MessageTaskStatuses status)
    {
        Status = status;
    }

    public void SetExpectSendTime()
    {
        if (!SendRules.IsCustom || string.IsNullOrEmpty(SendRules.CronExpression))
        {
            ExpectSendTime = DateTimeOffset.UtcNow;
        }
        else
        {
            var cronExpression = new CronExpression(SendRules.CronExpression);
            cronExpression.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
            var nextExcuteTime = cronExpression.GetNextValidTimeAfter(DateTimeOffset.UtcNow);
            ExpectSendTime = nextExcuteTime;
        }
    }

    public void SetJobId(Guid jobId)
    {
        SchedulerJobId = jobId;
    }

    public void SetSystemId(string systemId)
    {
        SystemId = systemId;
    }

    public int GetSendingCount(List<MessageReceiverUser> receiverUsers)
    {
        var sendingCount = (int)SendRules.SendingCount;
        if (sendingCount == 0)
        {
            sendingCount = receiverUsers.Count;
        }
        return sendingCount;
    }

    public long GetHistoryCount(List<MessageReceiverUser> receiverUsers)
    {
        var totalCount = receiverUsers.Count;

        var sendingCount = GetSendingCount(receiverUsers);

        var historyNum = (long)Math.Ceiling((double)totalCount / sendingCount);

        if (ReceiverType == ReceiverTypes.Broadcast)
        {
            historyNum = 1;
        }

        return historyNum;
    }

    public List<MessageReceiverUser> GetHistoryReceiverUsers(List<MessageReceiverUser> receiverUsers, int historyNum, int sendingCount)
    {
        return receiverUsers.Skip(historyNum * sendingCount).Take(sendingCount).ToList(); ;
    }

    public bool IsAppInWebsiteMessage => ChannelType?.Id == ChannelType.App.Id && ExtraProperties.GetProperty<bool>(BusinessConsts.IS_WEBSITE_MESSAGE);

    public string AppIntentUrl => ExtraProperties.GetProperty<string>(BusinessConsts.INTENT_URL);

    public string IsApnsProduction => ExtraProperties.GetProperty<string>(BusinessConsts.IS_APNS_PRODUCTION);

    public bool IsUniformContent => EntityType == MessageEntityTypes.Ordinary || !Receivers.Any(x => x.Variables.Any());
}