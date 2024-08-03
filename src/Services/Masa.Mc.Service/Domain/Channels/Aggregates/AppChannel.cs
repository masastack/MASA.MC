// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.Channels.Aggregates;

public class AppChannel : Entity<Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;
    public string Color { get; protected set; } = string.Empty;
    public string Code { get; protected set; } = string.Empty;
    public ChannelType Type { get; protected set; } = default!;
    public string Scheme { get; protected set; } = string.Empty;
    public string SchemeField { get; protected set; } = string.Empty;
}
