// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Unsubscriptions.EventHandler;

public class UnsubscribedBySmsInboundEventHandler
{
    [EventHandler]
    public Task HandleEventAsync(UnsubscribedBySmsInboundDomainEvent @event)
    {
        return Task.CompletedTask;
    }
}
