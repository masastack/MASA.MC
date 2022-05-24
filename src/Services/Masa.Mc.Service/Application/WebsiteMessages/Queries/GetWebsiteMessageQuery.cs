// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages.Queries;

public record GetWebsiteMessageQuery(Guid WebsiteMessageId) : Query<WebsiteMessageDto>
{
    public override WebsiteMessageDto Result { get; set; } = new();
}
