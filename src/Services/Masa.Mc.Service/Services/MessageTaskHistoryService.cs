// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskHistoryService : ServiceBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public MessageTaskHistoryService(IServiceCollection services) : base("api/message-task-history")
    {
        MapGet(GetListAsync, string.Empty);
    }

    public async Task<PaginatedListDto<MessageTaskHistoryDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? messageTaskId, [FromQuery] MessageTaskHistoryStatuses? status, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTaskHistoryInputDto(filter, messageTaskId, status, startTime, endTime, sorting, page, pagesize);
        var query = new GetMessageTaskHistoryListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskHistoryDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTaskHistoryQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task WithdrawnAsync(IEventBus eventBus, WithdrawnMessageTaskHistoryInputDto inputDto)
    {
        var command = new WithdrawnMessageTaskHistoryCommand(inputDto);
        await eventBus.PublishAsync(command);
    }
}
