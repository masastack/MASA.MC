// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageRecordService : ServiceBase
{
    public MessageRecordService(IServiceCollection services) : base(services, "api/message-record")
    {
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
    }

    public async Task<PaginatedListDto<MessageRecordDto>> GetListAsync(IEventBus eventbus, [FromQuery] bool? success, [FromQuery] MessageRecordTimeTypes? timeType,
       [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var inputDto = new GetMessageRecordInputDto(filter, success, timeType, startTime, endTime, sorting, page, pagesize);
        var query = new GetListMessageRecordQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageRecordDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageRecordQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
