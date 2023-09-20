// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages.EventHandler;

public class RemoveWebsiteMessageCacheEventHandler
{
    private readonly IDistributedCacheClient _cacheClient;
    public RemoveWebsiteMessageCacheEventHandler(IDistributedCacheClient cacheClient)
    {
        _cacheClient = cacheClient;
    }
    [EventHandler]
    public async Task HandleEvent(RemoveWebsiteMessageCacheEvent eto)
    {
        var cacheKey = CacheKeys.GET_NOTICE_LIST + "-" + eto.UserId;
        await _cacheClient.RemoveAsync<List<WebsiteMessageDto>>(cacheKey);
    }
}
