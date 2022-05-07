// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Events;

public record SendWebsiteMessageEvent : SendMessageEvent
{
    public SendWebsiteMessageEvent(Guid channelId, MessageData messageData, MessageTaskHistory messageTaskHistory)
        : base(channelId, messageData, messageTaskHistory)
    {

    }
}