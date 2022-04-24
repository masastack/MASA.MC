namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskService : ServiceBase
{
    public MessageTaskService(IServiceCollection services) : base(services, "api/message-task")
    {
        MapPost(CreateAsync, string.Empty);
        MapPut(UpdateAsync, "{id}");
        MapDelete(DeleteAsync, "{id}");
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapPost(ExecuteAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityType? entityType, [FromQuery] bool? isEnabled, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var input = new GetMessageTaskInput(filter, channelId, entityType, isEnabled, sorting, page, pagesize);
        var query = new GetListMessageTaskQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTaskQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] MessageTaskCreateUpdateDto input)
    {
        var command = new CreateMessageTaskCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] MessageTaskCreateUpdateDto input)
    {
        var command = new UpdateMessageTaskCommand(id, input);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteMessageTaskCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task ExecuteAsync(IEventBus eventBus, ExecuteMessageTaskInput input)
    {
        var command = new ExecuteMessageTaskCommand(input);
        await eventBus.PublishAsync(command);
    }
}
