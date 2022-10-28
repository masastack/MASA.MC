// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.WebsiteMessages;

public class WebsiteMessageCursorCommandHandler
{
    private readonly WebsiteMessageCursorDomainService _domainService;
    private readonly IDistributedCacheClient _cacheClient;
    private readonly ILogger<WebsiteMessageCursorCommandHandler> _logger;
    private readonly IUserContext _userContext;

    public WebsiteMessageCursorCommandHandler(WebsiteMessageCursorDomainService domainService
        , IDistributedCacheClient cacheClient
        , ILogger<WebsiteMessageCursorCommandHandler> logger
        , IUserContext userContext)
    {
        _domainService = domainService;
        _cacheClient = cacheClient;
        _logger = logger;
        _userContext = userContext;
    }

    [EventHandler]
    public virtual async Task CheckAsync(CheckWebsiteMessageCursorCommand command)
    {
        var currentUserId = _userContext.GetUserId<Guid>();

        if (currentUserId == default)
        {
            return;
        }

        var cacheKey = $"{CacheKeys.MESSAGE_CURSOR_CHECK_COUNT}_{currentUserId}";

        var checkCount = await _cacheClient.HashIncrementAsync(cacheKey);
        await _cacheClient.KeyExpireAsync<long>(cacheKey, DateTimeOffset.Now.AddMinutes(1));

        if (checkCount > 1)
        {
            return;
        }
        try
        {
            await _domainService.CheckAsync(currentUserId);
            await _cacheClient.RemoveAsync<long>(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "CheckAsync");
            await _cacheClient.RemoveAsync<long>(cacheKey);
        }
    }
}