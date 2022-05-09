 // Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
        MapPost(SendAsync);
        MapPost(SendTestAsync);
        MapPost(EnabledAsync);
        MapPost(DisableAsync);
        MapGet(GenerateImportTemplateAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityTypes? entityType, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeTypes? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var inputDto = new GetMessageTaskInputDto(filter, channelId, entityType, isEnabled, timeType, startTime, endTime, sorting, page, pagesize);
        var query = new GetListMessageTaskQuery(inputDto);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTaskQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] MessageTaskUpsertDto inputDto)
    {
        switch (inputDto.EntityType)
        {
            case MessageEntityTypes.Ordinary:
                var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(inputDto);
                await eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityTypes.Template:
                var templateCommand = new CreateTemplateMessageTaskCommand(inputDto);
                await eventBus.PublishAsync(templateCommand);
                break;
            default:
                throw new UserFriendlyException("unknown message task type");
        }

    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] MessageTaskUpsertDto inputDto)
    {
        switch (inputDto.EntityType)
        {
            case MessageEntityTypes.Ordinary:
                var ordinaryCommand = new UpdateOrdinaryMessageTaskCommand(id, inputDto);
                await eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityTypes.Template:
                var templateCommand = new UpdateTemplateMessageTaskCommand(id, inputDto);
                await eventBus.PublishAsync(templateCommand);
                break;
            default:
                throw new UserFriendlyException("unknown message task type");
        }
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteMessageTaskCommand(id);
        await eventBus.PublishAsync(command);
    }

    public async Task SendAsync(IEventBus eventBus, SendMessageTaskInputDto inputDto)
    {
        var command = new SendMessageTaskCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task SendTestAsync(IEventBus eventBus, SendTestMessageTaskInputDto inputDto)
    {
        var command = new SendTestMessageTaskCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task EnabledAsync(IEventBus eventBus, EnabledMessageTaskInputDto inputDto)
    {
        var command = new EnabledMessageTaskCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task DisableAsync(IEventBus eventBus, DisableMessageTaskInputDto inputDto)
    {
        var command = new DisableMessageTaskCommand(inputDto);
        await eventBus.PublishAsync(command);
    }

    public async Task<FileStreamResult> GenerateImportTemplateAsync(IEventBus eventBus)
    {
        var query = new GenerateImportTemplateQuery();
        await eventBus.PublishAsync(query);
        var memoryStream = new MemoryStream(query.Result);
        return new FileStreamResult(memoryStream, "text/csv")
        {
            FileDownloadName = "ImportTemplate.csv"
        };
    }
}
