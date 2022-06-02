// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates;

public class WebsiteMessage : FullAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }

    public AppChannel Channel { get; protected set; } = default!;

    public Guid UserId { get; protected set; }

    public string Title { get; protected set; } = string.Empty;

    public string Content { get; protected set; } = string.Empty;

    public string LinkUrl { get; protected set; } = string.Empty;

    public DateTimeOffset SendTime { get; protected set; }

    public bool IsRead { get; set; }

    public DateTimeOffset? ReadTime { get; set; }

    public WebsiteMessage(Guid channelId, Guid userId, string title, string content, string linkUrl, DateTimeOffset sendTime) : this(channelId, userId, title, content, linkUrl, sendTime, false, null)
    {
    }

    public WebsiteMessage(Guid channelId, Guid userId, string title, string content, string linkUrl, DateTimeOffset sendTime, bool isRead, DateTimeOffset? readTime)
    {
        ChannelId = channelId;
        UserId = userId;
        Title = title;
        Content = content;
        LinkUrl = linkUrl;
        SendTime = sendTime;
        IsRead = isRead;
        ReadTime = readTime;
    }

    public void SetRead()
    {
        IsRead = true;
        ReadTime = DateTimeOffset.Now;
    }
}
