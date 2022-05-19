// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Events;

public record AddMessageTaskHistoryEvent(MessageTask MessageTask, ReceiverTypes ReceiverType, MessageTaskSelectReceiverTypes selectReceiverType, List<MessageTaskReceiver> Receivers, ExtraPropertyDictionary SendRules, DateTime? SendTime, string Sign, ExtraPropertyDictionary Variables) : DomainEvent
{
}
