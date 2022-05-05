// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class ChannelService : ServiceBase
{
    public ChannelService(IServiceCollection services) : base(services, "api/channel")
    {
        MapPost(CreateAsync, string.Empty);
        MapPut(UpdateAsync, "{id}");
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapGet(FindByCodeAsync);
        MapGet(GetListByTypeAsync);
    }

    public async Task<PaginatedListDto<ChannelDto>> GetListAsync(IEventBus eventbus, [FromQuery] ChannelTypes? type, [FromQuery] string displayName = "", [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var inputDto = new GetChannelInputDto(filter, type, displayName, sorting, page, pagesize);
        var query = new GetListChannelQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    //public async Task<PaginatedListDto<ChannelDto>> GetListAsync([FromServices] IEventBus eventBus, GetChannelInput inputDto)
    //{
    //    var query = new GetListChannelQuery(inputDto);
    //    await eventBus.PublishAsync(query);
    //    return query.Result;
    //}

    public async Task<ChannelDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetChannelQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] ChannelCreateUpdateDto inputDto)
    {
        var command = new CreateChannelCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] ChannelCreateUpdateDto inputDto)
    {
        var command = new UpdateChannelCommand(id, inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteChannelCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task<ChannelDto> FindByCodeAsync(IEventBus eventBus, string code)
    {
        var query = new FindChannelByCodeQuery(code);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<ChannelDto>> GetListByTypeAsync(IEventBus eventBus, ChannelTypes type)
    {
        var query = new GetListByTypeQuery(type);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
