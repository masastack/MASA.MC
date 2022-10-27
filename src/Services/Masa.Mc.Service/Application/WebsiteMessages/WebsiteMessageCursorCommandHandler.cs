﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System;
using Masa.BuildingBlocks.Caching;

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

        var cacheKey = $"{CacheKeys.MESSAGE_CURSOR_CHECK_COUNT}_{currentUserId}";

        var checkCount = await _cacheClient.HashIncrementAsync(cacheKey, 1, options =>
        {
            options.CacheKeyType = CacheKeyType.None;
        });

        if (checkCount > 1)
        {
            return;
        }
        try
        {
            await _domainService.CheckAsync(currentUserId);
            await _cacheClient.RemoveAsync(cacheKey);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex, "CheckAsync");
            await _cacheClient.RemoveAsync(cacheKey);
        }
    }
}