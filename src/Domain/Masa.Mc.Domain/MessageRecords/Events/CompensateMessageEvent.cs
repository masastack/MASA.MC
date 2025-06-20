// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageRecords.Events;

public record CompensateMessageEvent(Guid UserId, string ChannelCode, string TemplateCode, ExtraPropertyDictionary Variables) : DomainEvent;
