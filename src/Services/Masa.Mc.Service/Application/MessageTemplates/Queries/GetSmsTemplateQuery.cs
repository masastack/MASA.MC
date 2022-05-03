// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetSmsTemplateQuery(Guid ChannelId,string TemplateCode) : Query<SmsTemplateDto>
{
    public override SmsTemplateDto Result { get; set; } = new();
}
