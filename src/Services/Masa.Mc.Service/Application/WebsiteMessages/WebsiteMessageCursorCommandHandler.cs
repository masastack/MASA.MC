// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCursorCommandHandler
{
    private readonly WebsiteMessageCursorDomainService _domainService;
    private readonly IDistributedCacheClient _cacheClient;
    private readonly IHubContext<NotificationsHub> _hubContext;

    public WebsiteMessageCursorCommandHandler(WebsiteMessageCursorDomainService domainService
        , IDistributedCacheClient cacheClient
        , IHubContext<NotificationsHub> hubContext)
    {
        _domainService = domainService;
        _cacheClient = cacheClient;
        _hubContext = hubContext;
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

        var onlineClients = _hubContext.Clients.Users(currentUserId.ToString());
        await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
    }
}