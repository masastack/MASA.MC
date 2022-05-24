// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Events;

public record WebsiteMessageCreatedDomainEvent(Guid UserId, DateTime CheckTime) : DomainEvent
{
}