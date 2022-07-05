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
        MapGet(GenerateReceiverImportTemplateAsync);
        MapPost(ImportReceiversAsync);
        MapGet(GetMessageTaskReceiverListAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityTypes? entityType, [FromQuery] bool? isDraft, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeTypes? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] MessageTaskStatuses? status, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTaskInputDto(filter, channelId, entityType, isDraft, isEnabled, timeType, startTime, endTime, status, sorting, page, pagesize);
        var query = new GetMessageTaskListQuery(inputDto);
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

    public async Task<byte[]> GenerateReceiverImportTemplateAsync(IEventBus eventBus, Guid? messageTemplatesId, ChannelTypes channelType)
    {
        var query = new GenerateReceiverImportTemplateQuery(messageTemplatesId, channelType);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<ImportResultDto<MessageTaskReceiverDto>> ImportReceiversAsync(IEventBus eventBus, ImportReceiversDto dto)
    {
        var command = new ImportReceiversCommand(dto);
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    public async Task<List<MessageTaskReceiverDto>> GetMessageTaskReceiverListAsync(IEventBus eventBus, [FromQuery] string filter = "")
    {
        var list = new List<MessageTaskReceiverDto>();

        var subjectInputDto = new GetSubjectInputDto(filter);
        var subjectQuery = new GetSubjectListQuery(subjectInputDto);
        await eventBus.PublishAsync(subjectQuery);
        var subjectList = subjectQuery.Result;
        list.AddRange(subjectList.Select(x => new MessageTaskReceiverDto
        {
            SubjectId = x.SubjectId,
            DisplayName = x.Name ?? x.DisplayName,
            Avatar = x.Avatar,
            PhoneNumber = x.PhoneNumber,
            Email = x.Email,
            Type = (MessageTaskReceiverTypes)(int)x.SubjectType
        }));

        var receiverGroupInputDto = new GetReceiverGroupInputDto(filter, "", 1, 99);
        var receiverGroupQuery = new GetReceiverGroupListQuery(receiverGroupInputDto);
        await eventBus.PublishAsync(receiverGroupQuery);
        var receiverGroupList = receiverGroupQuery.Result.Result;
        list.AddRange(receiverGroupList.Select(x => new MessageTaskReceiverDto
        {
            SubjectId = x.Id,
            DisplayName = x.DisplayName,
            Type = MessageTaskReceiverTypes.Group
        }));

        return list;
    }
}