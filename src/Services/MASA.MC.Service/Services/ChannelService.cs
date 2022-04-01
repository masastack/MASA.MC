using MASA.MC.Contracts.Admin.Dtos;
using MASA.MC.Contracts.Admin.Dtos.Channels;
using MASA.MC.Contracts.Admin.Enums.Channels;
using MASA.MC.Service.Admin.Application.Channels.Commands;
using MASA.MC.Service.Admin.Application.Channels.Queries;
using Microsoft.AspNetCore.Mvc;

namespace MASA.MC.Service.Services
{
    
    public class ChannelService : ServiceBase
    {
        public ChannelService(IServiceCollection services) : base(services, "api/channel")
        {
            MapPost(CreateAsync,string.Empty);
            MapPut(UpdateAsync, "{id}");
            MapDelete(DeleteAsync, "{id}");
            MapGet(GetAsync, "{id}");
            MapGet(GetListAsync, string.Empty);
        }
        public async Task<PaginatedListDto<ChannelDto>> GetListAsync([FromServices] IEventBus eventbus, [FromQuery] ChannelType type, [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
        {
            var input = new GetChannelInput(page, pagesize, type);
            var query = new GetListChannelQuery(input);
            await eventbus.PublishAsync(query);
            return query.Result;
        }
        //public async Task<PaginatedListDto<ChannelDto>> GetListAsync([FromServices] IEventBus eventBus, [FromQuery] GetChannelInput input)
        //{
        //    var query = new GetListChannelQuery(input);
        //    await eventBus.PublishAsync(query);
        //    return query.Result;
        //}
        public async Task<ChannelDto> GetAsync([FromServices] IEventBus eventBus, Guid id)
        {
            var query = new GetChannelQuery(id);
            await eventBus.PublishAsync(query);
            return query.Result;
        }
        public async Task CreateAsync([FromServices] IEventBus eventBus, [FromBody] ChannelCreateUpdateDto input)
        {
            var command = new CreateChannelCommand(input);
            await eventBus.PublishAsync(command);
        }

        public async Task UpdateAsync([FromServices] IEventBus eventBus, Guid id, [FromBody] ChannelCreateUpdateDto input)
        {
            var command = new UpdateChannelCommand(id, input);
            await eventBus.PublishAsync(command);
        }
        public async Task DeleteAsync([FromServices] IEventBus eventBus, Guid id)
        {
            var command = new DeleteChannelCommand(id);
            await eventBus.PublishAsync(command); 
        }

    }
}