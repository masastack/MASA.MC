// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Aggregates;

public class ChannelUser : ValueObject
{
    public ChannelTypes ChannelType { get; protected set; }

    public string ChannelUserIdentity { get; protected set; }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return ChannelType;
        yield return ChannelUserIdentity;
    }

    public ChannelUser(ChannelTypes channelType, string channelUserIdentity)
    {
        ChannelType = channelType;
        ChannelUserIdentity = channelUserIdentity;
    }
}
