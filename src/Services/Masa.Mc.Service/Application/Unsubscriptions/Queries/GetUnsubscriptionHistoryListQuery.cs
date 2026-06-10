// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.Unsubscriptions.Queries;

public record GetUnsubscriptionHistoryListQuery(GetUnsubscriptionHistoryInputDto Input) : Query<PaginatedListDto<UnsubscriptionHistoryDto>>
{
    public override PaginatedListDto<UnsubscriptionHistoryDto> Result { get; set; } = default!;
}
