﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageInfos.Aggregates;

public class MessageInfo : FullAggregateRoot<Guid, Guid>
{
    public MessageContent MessageContent { get; protected set; } = default!;

    public int Type { get;  set; }

    private MessageInfo() { }

    public MessageInfo(MessageContent messageContent)
    {
        MessageContent = messageContent;
    }
}
