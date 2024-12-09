// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Aggregates;

public class MessageReceiverUser : ValueObject
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public ExtraPropertyDictionary Variables { get; set; } = new();

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return UserId;
        yield return ChannelUserIdentity;

        foreach (var variable in Variables.OrderBy(x => x.Key))
        {
            yield return variable.Key;
            yield return variable.Value?.ToString() ?? string.Empty;
        }
    }

    private MessageReceiverUser() { }

    public MessageReceiverUser(Guid userId, string channelUserIdentity, ExtraPropertyDictionary variables)
    {
        UserId = userId;
        ChannelUserIdentity = channelUserIdentity;
        Variables = variables;
    }
}
