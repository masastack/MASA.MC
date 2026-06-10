// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Unsubscriptions.Events;

public record UnsubscribedBySmsInboundDomainEvent(
    Guid UnsubscriptionId,
    Guid ChannelId,
    Guid UserId,
    string ChannelUserIdentity,
    string ScopeRefId,
    string Keyword) : DomainEvent;
