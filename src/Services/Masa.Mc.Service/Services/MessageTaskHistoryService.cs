﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskHistoryService : ServiceBase
{
    public MessageTaskHistoryService(IServiceCollection services) : base(services, "api/message-task-history")
    {
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
    }

    public async Task<PaginatedListDto<MessageTaskHistoryDto>> GetListAsync(IEventBus eventbus, [FromQuery] MessageTaskHistoryStatuses? status, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var inputDto = new GetMessageTaskHistoryInputDto(status, startTime, endTime, sorting, page, pagesize);
        var query = new GetListMessageTaskHistoryQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskHistoryDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTaskHistoryQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
