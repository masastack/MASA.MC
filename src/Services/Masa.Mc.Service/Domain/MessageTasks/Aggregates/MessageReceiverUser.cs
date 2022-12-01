// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Aggregates;

public class MessageReceiverUser : ValueObject
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return UserId;
        yield return ChannelUserIdentity;
    }

    private MessageReceiverUser() { }

    public MessageReceiverUser(Guid userId, string channelUserIdentity, ExtraPropertyDictionary variables)
    {
        UserId = userId;
        ChannelUserIdentity = channelUserIdentity;
        Variables = variables;
    }
}
