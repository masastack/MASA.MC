// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions.Queries;

public record GetUnsubscriptionQuery(Guid Id) : Query<UnsubscriptionDetailDto>
{
    public override UnsubscriptionDetailDto Result { get; set; } = default!;
}
