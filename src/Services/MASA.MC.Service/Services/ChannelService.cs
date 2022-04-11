namespace MASA.MC.Service.Services;

public class ChannelService : ServiceBase
{
    public ChannelService(IServiceCollection services) : base(services, "api/channel")
    {
        MapPost(CreateAsync,string.Empty);
        MapPut(UpdateAsync, "{id}");
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapGet(FindByCodeAsync);
        MapGet(GetListByTypeAsync);
    }

    public async Task<PaginatedListDto<ChannelDto>> GetListAsync(IEventBus eventbus, [FromQuery] string filter,[FromQuery] ChannelType? type, [FromQuery] string displayName="", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var input = new GetChannelInput(filter,type, displayName, sorting, page, pagesize);
        var query = new GetListChannelQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    //public async Task<PaginatedListDto<ChannelDto>> GetListAsync([FromServices] IEventBus eventBus, GetChannelInput input)
    //{
    //    var query = new GetListChannelQuery(input);
    //    await eventBus.PublishAsync(query);
    //    return query.Result;
    //}

    public async Task<ChannelDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetChannelQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] ChannelCreateUpdateDto input)
    {
        var command = new CreateChannelCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] ChannelCreateUpdateDto input)
    {
        var command = new UpdateChannelCommand(id, input);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteChannelCommand(id);
        await eventBus.PublishAsync(command); 
    }

    public async Task<ChannelDto> FindByCodeAsync(IEventBus eventBus, string code)
    {
        var query = new FindByCodeChannelQuery(code);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<List<ChannelDto>> GetListByTypeAsync(IEventBus eventBus, ChannelType type)
    {
        var query = new GetListByTypeQuery(type);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
