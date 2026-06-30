// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.QueryModels;

public class UnsubscriptionTimelineQueryModel : Masa.BuildingBlocks.Ddd.Domain.Entities.Auditing.AuditEntity<Guid, Guid>
{
    public Guid UnsubscriptionId { get; set; }

    public UnsubscriptionTimelineActions Action { get; set; }

    public string Detail { get; set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; set; }
}
