namespace Masa.Mc.Service.Services;

public class ReceiverGroupService : ServiceBase
{
    public ReceiverGroupService(IServiceCollection services) : base(services, "api/receiver-group")
    {
        MapPost(CreateAsync, string.Empty);
        MapPut(UpdateAsync, "{id}");
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
    }

    public async Task<PaginatedListDto<ReceiverGroupDto>> GetListAsync(IEventBus eventbus, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var input = new GetReceiverGroupInput(filter, sorting, page, pagesize);
        var query = new GetListReceiverGroupQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<ReceiverGroupDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetReceiverGroupQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] ReceiverGroupCreateUpdateDto input)
    {
        var command = new CreateReceiverGroupCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] ReceiverGroupCreateUpdateDto input)
    {
        var command = new UpdateReceiverGroupCommand(id, input);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteReceiverGroupCommand(id);
        await eventBus.PublishAsync(command);
    }
}
