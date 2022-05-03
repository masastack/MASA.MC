// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.ReceiverGroups.Queries;

public record GetReceiverGroupQuery(Guid ReceiverGroupId) : Query<ReceiverGroupDto>
{
    public override ReceiverGroupDto Result { get; set; } = new();
}
