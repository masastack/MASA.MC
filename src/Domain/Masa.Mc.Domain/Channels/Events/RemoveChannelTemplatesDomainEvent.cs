﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Channels.Events;

public record RemoveChannelTemplatesDomainEvent(Guid ChannelId) : DomainEvent
{
}