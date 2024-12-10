// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.WebsiteMessages.Aggregates;

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

    public bool IsWithdrawn { get; protected set; }

    public Guid MessageTaskHistoryId { get; protected set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    public List<WebsiteMessageTag> Tags { get; protected set; } = new();


    public WebsiteMessage(Guid messageTaskHistoryId, Guid channelId, Guid userId, string title, string content, string linkUrl, DateTimeOffset sendTime, ExtraPropertyDictionary extraProperties) : this(messageTaskHistoryId, channelId, userId, title, content, linkUrl, sendTime, false, null, extraProperties)
    {
    }

    public WebsiteMessage(Guid messageTaskHistoryId, Guid channelId, Guid userId, string title, string content, string linkUrl, DateTimeOffset sendTime, bool isRead, DateTimeOffset? readTime, ExtraPropertyDictionary extraProperties)
    {
        MessageTaskHistoryId = messageTaskHistoryId;
        ChannelId = channelId;
        UserId = userId;
        Title = title;
        Content = content;
        LinkUrl = linkUrl;
        SendTime = sendTime;
        IsRead = isRead;
        ReadTime = readTime;
        ExtraProperties = extraProperties;

        AddTag();

        if (userId != Guid.Empty)
        {
            AddDomainEvent(new RemoveWebsiteMessageCacheEvent(userId));
        }
    }

    public void SetRead()
    {
        IsRead = true;
        ReadTime = DateTimeOffset.UtcNow;
    }

    public void SetWithdraw()
    {
        IsWithdrawn = true;
    }

    public void AddTag()
    {
        var tag = ExtraProperties.GetProperty<string>("Tag");

        if (!string.IsNullOrEmpty(tag))
        {
            Tags.Add(new WebsiteMessageTag(tag, ChannelId, UserId));
        }
    }
}
