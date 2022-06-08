// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Events;

public record SendMessageEvent(Guid ChannelId, MessageData MessageData, MessageTaskHistory MessageTaskHistory) : Event
{
}
