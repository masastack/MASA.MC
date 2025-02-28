﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.WebsiteMessages.Aggregates;

public class WebsiteMessageCursor : FullAggregateRoot<Guid, Guid>
{
    public Guid UserId { get; protected set; }

    public DateTimeOffset UpdateTime { get; protected set; }

    public WebsiteMessageCursor(Guid userId, DateTimeOffset updateTime)
    {
        UserId = userId;
        UpdateTime = updateTime;
    }

    public virtual void Update()
    {
        UpdateTime = DateTimeOffset.UtcNow;
    }
}