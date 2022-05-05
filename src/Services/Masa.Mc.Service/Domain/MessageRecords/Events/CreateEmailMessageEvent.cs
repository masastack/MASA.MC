// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Events;

public record CreateEmailMessageEvent : CreateMessageEvent
{
    public CreateEmailMessageEvent(Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, IEnumerable<Guid> userIds)
        : base(channelId, messageTaskId, messageTaskHistoryId, userIds)
    {

    }
    public CreateEmailMessageEvent(Guid channelId, Guid messageTaskId, Guid messageTaskHistoryId, Guid userId)
        : base(channelId, messageTaskId, messageTaskHistoryId, userId)
    {

    }
}
