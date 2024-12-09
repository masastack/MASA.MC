// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Aggregates;

public class MessageTaskReceiver : ValueObject
{
    public Guid SubjectId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public MessageTaskReceiverTypes Type { get; set; }

    public ExtraPropertyDictionary Variables { get; set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return SubjectId;
        yield return ChannelUserIdentity;
        yield return Type;
    }

    public MessageTaskReceiver(Guid subjectId, string channelUserIdentity, MessageTaskReceiverTypes type, ExtraPropertyDictionary variables)
    {
        SubjectId = subjectId;
        ChannelUserIdentity = channelUserIdentity;
        Type = type;
        Variables = variables;
    }
}