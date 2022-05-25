// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class WebsiteMessageService : ServiceBase
{
    public WebsiteMessageService(IServiceCollection services) : base(services, "api/website-message")
    {
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetChannelListAsync);
        MapPost(SetAllReadAsync);
        MapPost(ReadAsync);
        MapPost(CheckAsync);
    }

    public async Task<PaginatedListDto<WebsiteMessageDto>> GetListAsync(IEventBus eventbus, [FromQuery] WebsiteMessageFilterType? filterType, [FromQuery] Guid? channelId, [FromQuery] bool? isRead, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetWebsiteMessageInputDto(filter, filterType, channelId, isRead, sorting, page, pagesize);
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

    public async Task<List<WebsiteMessageChannelListDto>> GetChannelListAsync(IEventBus eventbus)
    {
        var query = new GetChannelListWebsiteMessageQuery();
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task SetAllReadAsync(IEventBus eventbus, [FromBody] GetWebsiteMessageInputDto inputDto)
    {
        var command = new SetAllReadWebsiteMessageCommand(inputDto);
        await eventbus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteWebsiteMessageCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task ReadAsync(IEventBus eventbus, [FromBody] ReadWebsiteMessageInputDto inputDto)
    {
        var command = new ReadWebsiteMessageCommand(inputDto);
        await eventbus.PublishAsync(command);
    }

    public async Task CheckAsync(IEventBus eventbus)
    {
        var command = new CheckWebsiteMessageCursorCommand();
        await eventbus.PublishAsync(command);
    }
}