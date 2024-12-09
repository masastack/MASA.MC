// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.WebsiteMessages.Aggregates;

public class WebsiteMessageTag : ValueObject
{
    public string Tag { get; set; } = string.Empty;

    public Guid ChannelId { get; protected set; }

    public Guid UserId { get; protected set; }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Tag;
    }

    public WebsiteMessageTag(string tag, Guid channelId, Guid userId)
    {
        Tag = tag;
        ChannelId = channelId;
        UserId = userId;
    }
}
