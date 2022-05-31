// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Events;

public record SmsTemplateSynchroIntegrationDomainEvent(Guid ChannelId) : IntegrationDomainEvent
{
    public override string Topic { get; set; } = nameof(SmsTemplateSynchroIntegrationDomainEvent);
}