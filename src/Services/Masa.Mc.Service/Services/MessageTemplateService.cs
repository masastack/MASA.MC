// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTemplateService : ServiceBase
{
    public MessageTemplateService(IServiceCollection services) : base("api/message-template")
    {
        MapGet(GetListAsync, string.Empty);
    }

    public async Task<PaginatedListDto<MessageTemplateDto>> GetListAsync(IEventBus eventbus, [FromQuery] ChannelTypes? channelType, [FromQuery] Guid? channelId, [FromQuery] MessageTemplateStatuses? status, [FromQuery] MessageTemplateAuditStatuses? auditStatus, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] int templateType, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTemplateInputDto(filter, channelType, channelId, status, auditStatus, startTime, endTime, templateType, sorting, page, pagesize);
        var query = new GetMessageTemplateListQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTemplateDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTemplateQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] MessageTemplateUpsertDto inputDto)
    {
        var command = new CreateMessageTemplateCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] MessageTemplateUpsertDto inputDto)
    {
        var command = new UpdateMessageTemplateCommand(id, inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteMessageTemplateCommand(id);
        await eventBus.PublishAsync(command);
    }
}
