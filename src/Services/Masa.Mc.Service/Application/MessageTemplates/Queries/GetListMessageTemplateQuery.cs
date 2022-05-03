// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetListMessageTemplateQuery(GetMessageTemplateInput Input) : Query<PaginatedListDto<MessageTemplateDto>>
{
    public override PaginatedListDto<MessageTemplateDto> Result { get; set; } = default!;

}
