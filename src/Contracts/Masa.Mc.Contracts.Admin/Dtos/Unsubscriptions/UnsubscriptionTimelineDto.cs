// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class UnsubscriptionTimelineDto
{
    public Guid Id { get; set; }

    public UnsubscriptionTimelineActions Action { get; set; }

    public string Detail { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }
}
