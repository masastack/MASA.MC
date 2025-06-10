// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Apps.Aggregates;

public class AppVendorConfig: FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; private set; }

    public AppVendor Vendor { get; private set; }

    public ExtraPropertyDictionary Options { get; protected set; } = new();

    public AppVendorConfig(Guid channelId, AppVendor vendor, ExtraPropertyDictionary options)
    {
        ChannelId = channelId;
        Vendor = vendor;
        Options = options;
    }

    public void UpdateOptions(ExtraPropertyDictionary options)
    {
        Options = options;
    }
}
