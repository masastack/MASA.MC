// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Events;

public record CreateMessageEvent(Guid ChannelId, MessageData MessageData, Guid MessageTaskHistoryId) : DomainEvent
{
    public MessageTaskHistory MessageTaskHistory { get; set; }

    public List<MessageReceiverUser> ReceiverUsers { get; set; }
}