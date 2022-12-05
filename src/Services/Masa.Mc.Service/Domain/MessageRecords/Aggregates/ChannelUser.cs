// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

public class ChannelUser : ValueObject
{
    public Guid UserId { get; set; }

    public string ChannelUserIdentity { get; set; } = string.Empty;

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return UserId;
        yield return ChannelUserIdentity;
    }

    public ChannelUser(Guid userId, string channelUserIdentity)
    {
        UserId = userId;
        ChannelUserIdentity = channelUserIdentity;
    }
}
