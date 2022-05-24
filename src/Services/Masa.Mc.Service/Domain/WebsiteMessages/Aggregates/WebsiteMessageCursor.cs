// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Aggregates;

public class WebsiteMessageCursor : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; protected set; }

    public DateTime UpdateTime { get; protected set; }

    public WebsiteMessageCursor(Guid userId, DateTime updateTime)
    {
        UserId = userId;
        UpdateTime = updateTime;
    }

    public virtual void Update()
    {
        UpdateTime = DateTime.UtcNow;
    }
}