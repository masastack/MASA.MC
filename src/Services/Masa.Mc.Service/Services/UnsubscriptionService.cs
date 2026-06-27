// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class UnsubscriptionService : ServiceBase
{
    public UnsubscriptionService() : base("api/unsubscription")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    [RoutePattern("", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<PaginatedListDto<UnsubscriptionDto>> GetListAsync(
        IEventBus eventBus,
        [FromQuery] Guid? channelId,
        [FromQuery] Guid? userId,
        [FromQuery] UnsubscriptionSource? source,
        [FromQuery] UnsubscriptionStatus? status,
        [FromQuery] DateTimeOffset? startTime,
        [FromQuery] DateTimeOffset? endTime,
        [FromQuery] string filter = "",
        [FromQuery] string channelUserIdentity = "",
        [FromQuery] string scopeRefId = "",
        [FromQuery] string keyword = "",
        [FromQuery] string sorting = "",
        [FromQuery] int page = 1,
        [FromQuery] int pagesize = 10)
    {
        var input = new GetUnsubscriptionInputDto(
            channelId,
            userId,
            filter,
            channelUserIdentity,
            scopeRefId,
            keyword,
            source,
            status,
            startTime,
            endTime,
            sorting,
            page,
            pagesize);
        var query = new GetUnsubscriptionListQuery(input);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("{id}", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<UnsubscriptionDetailDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetUnsubscriptionQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("channel-user-identities", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<ChannelUserIdentityUnsubscriptionItemDto>> GetChannelUserIdentityUnsubscriptionsAsync(
        IEventBus eventBus,
        [FromQuery] string channelCode,
        [FromQuery] string channelUserIdentity)
    {
        var query = new GetChannelUserIdentityUnsubscriptionsQuery(channelCode, channelUserIdentity);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("channel-user-identities/unsubscriptions", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task AddChannelUserIdentityToBlacklistAsync(
        IEventBus eventBus,
        [FromBody] AddChannelUserIdentityToUnsubscriptionBlacklistInputDto input)
    {
        var command = new AddChannelUserIdentityToUnsubscriptionBlacklistCommand(input);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("channel-user-identities/unsubscriptions", StartWithBaseUri = true, HttpMethod = "Delete")]
    public async Task RemoveChannelUserIdentityFromBlacklistAsync(
        IEventBus eventBus,
        [FromBody] RemoveChannelUserIdentityFromUnsubscriptionBlacklistInputDto input)
    {
        var command = new RemoveChannelUserIdentityFromUnsubscriptionBlacklistCommand(input);
        await eventBus.PublishAsync(command);
    }
}
