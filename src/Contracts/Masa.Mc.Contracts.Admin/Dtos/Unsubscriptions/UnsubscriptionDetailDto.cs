// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class UnsubscriptionDetailDto : UnsubscriptionDto
{
    public List<UnsubscriptionTimelineDto> Timelines { get; set; } = new();
}
