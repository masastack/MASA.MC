namespace Masa.Mc.Service.Admin.Services;

public class MessageInfoService : ServiceBase
{
    public MessageInfoService(IServiceCollection services) : base(services, "api/message-info")
    {
        MapGet(GetAsync, "{id}");
    }

    public async Task<MessageInfoDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageInfoQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
