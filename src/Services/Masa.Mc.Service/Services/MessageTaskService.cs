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
        MapPost(WithdrawnHistoryAsync);
        MapPost(EnabledAsync);
        MapPost(DisableAsync);
        MapGet(GenerateImportTemplateAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityType? entityType, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeType? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 20)
    {
        var input = new GetMessageTaskInput(filter, channelId, entityType, isEnabled, timeType, startTime, endTime, sorting, page, pagesize);
        var query = new GetListMessageTaskQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new GetMessageTaskQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] MessageTaskCreateUpdateDto input)
    {
        switch (input.EntityType)
        {
            case MessageEntityType.Ordinary:
                var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(input);
                await eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityType.Template:
                var templateCommand = new CreateTemplateMessageTaskCommand(input);
                await eventBus.PublishAsync(templateCommand);
                break;
            default:
                throw new UserFriendlyException("unknown message task type");
        }

    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] MessageTaskCreateUpdateDto input)
    {
        switch (input.EntityType)
        {
            case MessageEntityType.Ordinary:
                var ordinaryCommand = new UpdateOrdinaryMessageTaskCommand(id, input);
                await eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityType.Template:
                var templateCommand = new UpdateTemplateMessageTaskCommand(id, input);
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

    public async Task SendAsync(IEventBus eventBus, SendMessageTaskInput input)
    {
        var command = new SendMessageTaskCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task SendTestAsync(IEventBus eventBus, SendTestMessageTaskInput input)
    {
        var command = new SendTestMessageTaskCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task WithdrawnHistoryAsync(IEventBus eventBus, WithdrawnMessageTaskHistoryInput input)
    {
        var command = new WithdrawnMessageTaskHistoryCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task EnabledAsync(IEventBus eventBus, EnabledMessageTaskInput input)
    {
        var command = new EnabledMessageTaskCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task DisableAsync(IEventBus eventBus, DisableMessageTaskInput input)
    {
        var command = new DisableMessageTaskCommand(input);
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
