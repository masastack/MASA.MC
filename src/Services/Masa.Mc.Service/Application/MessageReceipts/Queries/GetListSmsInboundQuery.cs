// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageReceipts.Queries;

public record GetListSmsInboundQuery(GetSmsInboundInputDto Input) : Query<PaginatedListDto<SmsInboundDto>>
{
    public override PaginatedListDto<SmsInboundDto> Result { get; set; } = default!;
}
