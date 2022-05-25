// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCursorCommandHandler
{
    private readonly WebsiteMessageCursorDomainService _domainService;

    public WebsiteMessageCursorCommandHandler(WebsiteMessageCursorDomainService domainService)
    {
        _domainService = domainService;
    }

    [EventHandler]
    public virtual async Task CheckAsync(CheckWebsiteMessageCursorCommand command)
    {
        var currentUserId = Guid.Parse("55206182-C619-4B5B-AAF8-1D460006257D");
        await _domainService.CheckAsync(currentUserId);
    }
}