// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Events;

public record SendWebsiteMessageEvent(Guid MessageTaskHistoryId) : DomainEvent
{

}