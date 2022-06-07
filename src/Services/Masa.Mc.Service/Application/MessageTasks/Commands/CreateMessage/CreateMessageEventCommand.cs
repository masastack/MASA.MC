// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Commands.CreateMessage;

public record CreateMessageEventCommand(Guid ChannelId, MessageData MessageData, Guid MessageTaskHistoryId) : Event
{
    public MessageTaskHistory MessageTaskHistory { get; set; }

    public List<MessageReceiverUser> ReceiverUsers { get; set; }
}