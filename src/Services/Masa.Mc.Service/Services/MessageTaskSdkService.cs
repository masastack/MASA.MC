// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskSdkService : ServiceBase
{
    public MessageTaskSdkService(IServiceCollection services) : base("api/message-task")
    {

    }

    public async Task SendOrdinaryMessageByInternalAsync(IEventBus eventBus, SendOrdinaryMessageByInternalInputDto inputDto)
    {
        var command = new SendOrdinaryMessageByInternalCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task SendTemplateMessageByInternalAsync(IEventBus eventBus, SendTemplateMessageByInternalInputDto inputDto)
    {
        var command = new SendTemplateMessageByInternalCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task SendOrdinaryMessageByExternalAsync(IEventBus eventBus, SendOrdinaryMessageByExternalInputDto inputDto)
    {
        var command = new SendOrdinaryMessageByExternalCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task SendTemplateMessageByExternalAsync(IEventBus eventBus, SendTemplateMessageByExternalInputDto inputDto)
    {
        var command = new SendTemplateMessageByExternalCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListBySdkAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityTypes? entityType, [FromQuery] bool? isDraft, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeTypes? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] MessageTaskStatuses? status, [FromQuery] MessageTaskSources? source, [FromQuery] string systemId, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTaskInputDto(filter, channelId, entityType, isDraft, isEnabled, timeType, startTime, endTime, status, source, systemId, sorting, page, pagesize);
        var query = new GetMessageTaskListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }
}
