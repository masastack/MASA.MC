// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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

    public async Task<PaginatedListDto<ReceiverGroupDto>> GetListAsync(IEventBus eventbus, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetReceiverGroupInputDto(filter, sorting, page, pagesize);
        var query = new GetReceiverGroupListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<ReceiverGroupDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetReceiverGroupQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] ReceiverGroupUpsertDto inputDto)
    {
        var command = new CreateReceiverGroupCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] ReceiverGroupUpsertDto inputDto)
    {
        var command = new UpdateReceiverGroupCommand(id, inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteReceiverGroupCommand(id);
        await eventBus.PublishAsync(command);
    }
}
