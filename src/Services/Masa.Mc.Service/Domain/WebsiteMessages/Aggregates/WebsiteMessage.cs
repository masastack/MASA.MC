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

    public DateTime SendTime { get; protected set; }

    public bool IsRead { get; set; }

    public DateTime? ReadTime { get; set; }

    public WebsiteMessage(Guid channelId, Guid userId, string title, string content, DateTime sendTime) : this(channelId, userId, title, content, sendTime, false, null)
    {
    }

    public WebsiteMessage(Guid channelId, Guid userId, string title, string content, DateTime sendTime, bool isRead, DateTime? readTime)
    {
        ChannelId = channelId;
        UserId = userId;
        Title = title;
        Content = content;
        SendTime = sendTime;
        IsRead = isRead;
        ReadTime = readTime;
    }

    private void SetRead()
    {
        IsRead = true;
        ReadTime = DateTime.UtcNow;
    }
}
