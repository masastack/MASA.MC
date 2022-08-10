﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageRecordService : ServiceBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public MessageRecordService(IServiceCollection services) : base(services, "api/message-record")
    {
        MapGet(GetAsync, "{id}");
        MapGet(GetListAsync, string.Empty);
        MapPost(RetryAsync);
        MapPost(UpdateUserAsync);
    }

    public async Task<PaginatedListDto<MessageRecordDto>> GetListAsync(IEventBus eventbus, Guid? channelId, [FromQuery] bool? success, [FromQuery] MessageRecordTimeTypes? timeType,
       [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] Guid? userId, [FromQuery] Guid? messageTemplateId, [FromQuery] Guid? messageTaskHistoryId, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageRecordInputDto(filter, channelId, success, timeType, startTime, endTime, userId, messageTemplateId, messageTaskHistoryId, sorting, page, pagesize);
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

    public async Task RetryAsync(IEventBus eventBus, RetryMessageRecordInputDto inputDto)
    {
        var command = new RetryMessageRecordCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    [Topic(DAPR_PUBSUB_NAME, nameof(UpdateMessageRecordUserEvent))]
    public async Task UpdateUserAsync(IEventBus eventbus, UpdateMessageRecordUserEvent @event)
    {
        var command = new UpdateMessageRecordUserCommand(@event.MessageRecordId);
        await eventbus.PublishAsync(command);
    }
}
