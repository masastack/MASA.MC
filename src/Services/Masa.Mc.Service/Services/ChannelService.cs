// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Services;

public class ChannelService : ServiceBase
{
    IEventBus _eventBus => GetRequiredService<IEventBus>();

    public ChannelService(IServiceCollection services) : base("api/channel")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        MapGet(GetListAsync, string.Empty);
        MapGet(FindByCodeAsync);
        MapGet(GetListByTypeAsync);
    }


    public async Task<PaginatedListDto<ChannelDto>> GetListAsync([FromQuery] ChannelTypes? type, [FromQuery] string displayName = "", [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetChannelInputDto(filter, type, displayName, sorting, page, pagesize);
        var query = new GetChannelListQuery(inputDto);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<ChannelDto> GetAsync(Guid id)
    {
        var query = new GetChannelQuery(id);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync([FromBody] ChannelUpsertDto inputDto)
    {
        var command = new CreateChannelCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(Guid id, [FromBody] ChannelUpsertDto inputDto)
    {
        inputDto.Scheme ??= string.Empty;
        var command = new UpdateChannelCommand(id, inputDto);
        await _eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(Guid id)
    {
        var command = new DeleteChannelCommand(id);
        await _eventBus.PublishAsync(command);
    }

    public async Task<ChannelDto> FindByCodeAsync(string code)
    {
        var query = new FindChannelByCodeQuery(code);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<ChannelDto>> GetListByTypeAsync(ChannelTypes type)
    {
        var query = new GetListByTypeQuery(type);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("{id}/vendor/{vendor}/config", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<VendorConfigDto> GetVendorConfigAsync([FromRoute] Guid id, [FromRoute] AppVendor vendor)
    {
        var query = new GetChannelVendorConfigQuery(id, vendor);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("{id}/vendors", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<AppVendorConfigDto>> GetVendorConfigsAsync([FromRoute] Guid id)
    {
        var query = new GetChannelVendorsQuery(id);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    [RoutePattern("{id}/vendor/{vendor}/config", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task SaveVendorConfigAsync([FromRoute] Guid id, [FromRoute] AppVendor vendor, [FromBody] VendorConfigUpsertDto vendorConfig)
    {
        var command = new SaveChannelVendorConfigCommand(id, vendor, vendorConfig.Options);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("{id}/vendors", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task SaveVendorsAsync([FromRoute] Guid id, [FromBody] List<AppVendorConfigDto> vendors)
    {
        var command = new SaveChannelVendorsCommand(id, vendors);
        await _eventBus.PublishAsync(command);
    }
}
