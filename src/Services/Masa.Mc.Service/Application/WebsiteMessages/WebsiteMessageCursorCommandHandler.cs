// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCursorCommandHandler
{
    private readonly WebsiteMessageCursorDomainService _domainService;
    private readonly IDistributedCacheClient _cacheClient;

    public WebsiteMessageCursorCommandHandler(WebsiteMessageCursorDomainService domainService
        , IDistributedCacheClient cacheClient)
    {
        _domainService = domainService;
        _cacheClient = cacheClient;
    }

    [EventHandler]
    public virtual async Task CheckAsync(CheckWebsiteMessageCursorCommand command)
    {
        var currentUserId = Guid.Parse(TempCurrentUserConsts.ID);
        var checkCount = await _cacheClient.HashIncrementAsync($"{CacheKeys.MESSAGE_CURSOR_CHECK_COUNT}_{currentUserId}");
        if (checkCount > 1)
        {
            return;
        }
        await _domainService.CheckAsync(currentUserId);
        await _cacheClient.RemoveAsync<int>($"{CacheKeys.MESSAGE_CURSOR_CHECK_COUNT}_{currentUserId}");
    }
}