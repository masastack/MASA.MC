// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetMessageTemplateQuery(Guid MessageTemplateId) : Query<MessageTemplateDto>
{
    public override MessageTemplateDto Result { get; set; } = new();
}
