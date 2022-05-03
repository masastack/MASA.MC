// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetSmsTemplateListByChannelIdQuery(Guid ChannelId) : Query<List<SmsTemplateDto>>
{
    public override List<SmsTemplateDto> Result { get; set; } = new();
}
