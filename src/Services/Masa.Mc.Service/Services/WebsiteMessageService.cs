// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class WebsiteMessageService : ServiceBase
{
    public WebsiteMessageService(IServiceCollection services) : base(services, "api/website-message")
    {
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapGet(GetChannelListAsync);
    }

    public async Task<PaginatedListDto<WebsiteMessageDto>> GetListAsync(IEventBus eventbus, [FromQuery] WebsiteMessageFilterType? filterType, [FromQuery] Guid? channelId, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetWebsiteMessageInputDto(filter, filterType, channelId, sorting, page, pagesize);
        var query = new GetListWebsiteMessageQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<WebsiteMessageDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetWebsiteMessageQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<WebsiteMessageDto>> GetChannelListAsync(IEventBus eventbus)
    {
        var query = new GetChannelListWebsiteMessageQuery();
        await eventbus.PublishAsync(query);
        return query.Result;
    }
}
