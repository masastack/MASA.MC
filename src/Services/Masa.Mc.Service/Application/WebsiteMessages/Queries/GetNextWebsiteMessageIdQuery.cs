// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages.Queries;

public record GetNextWebsiteMessageIdQuery(Guid WebsiteMessageId) : Query<Guid>
{
    public override Guid Result { get; set; } = new();
}
