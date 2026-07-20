// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetWeixinMiniProgramTemplateListQuery(Guid ChannelId) : Query<List<WeixinMiniProgramTemplateDto>>
{
    public override List<WeixinMiniProgramTemplateDto> Result { get; set; } = new();
}
