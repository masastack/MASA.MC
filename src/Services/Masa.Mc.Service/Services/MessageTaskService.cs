// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskService : ServiceBase
{
    public MessageTaskService(IServiceCollection services) : base("api/message-task")
    {
        MapGet(GetListAsync, string.Empty);
        MapGet(GenerateReceiverImportTemplateAsync);
        MapGet(GetMessageTaskReceiverListAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(IEventBus eventbus, [FromQuery] Guid? channelId, [FromQuery] MessageEntityTypes? entityType, [FromQuery] bool? isDraft, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeTypes? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] MessageTaskStatuses? status, [FromQuery] MessageTaskSources? source, [FromQuery] string systemId, [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTaskInputDto(filter, channelId, entityType, isDraft, isEnabled, timeType, startTime, endTime, status, source, systemId, sorting, page, pagesize);
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


    [RoutePattern("{id}/enabled/{isEnabled}", StartWithBaseUri = true, HttpMethod = "Put")]
    public async Task SetIsEnabledAsync(IEventBus eventBus, Guid id, bool isEnabled)
    {
        if (isEnabled)
        {
            var command = new EnabledMessageTaskCommand(id);
            await eventBus.PublishAsync(command);
        }
        else
        {
            var command = new DisableMessageTaskCommand(id);
            await eventBus.PublishAsync(command);
        }
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

    public async Task<List<MessageTaskReceiverDto>> GetMessageTaskReceiverListAsync(IEventBus eventBus, IAuthClient authClient,[FromQuery] string filter = "")
    {
        var list = new List<MessageTaskReceiverDto>();
        var subjectList = await authClient.SubjectService.GetListAsync(filter);
        list.AddRange(subjectList.Select(x => new MessageTaskReceiverDto
        {
            SubjectId = x.SubjectId,
            DisplayName = x.DisplayName ?? string.Empty,
            Avatar = x.Avatar ?? string.Empty,
            PhoneNumber = x.PhoneNumber ?? string.Empty,
            Email = x.Email ?? string.Empty,
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

    public async Task<long> ResolveReceiversCountAsync(IEventBus eventBus, List<MessageTaskReceiverDto> dto)
    {
        var query = new ResolveReceiversCountQuery(dto);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task ExecuteAsync(IEventBus eventBus, Guid messageTaskId, Guid taskId)
    {
        var query = new ExecuteMessageTaskEvent(messageTaskId, false, default, taskId);
        await eventBus.PublishAsync(query);
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

    [RoutePattern("{id}/Withdrawn", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task WithdrawnAsync(IEventBus eventBus, Guid id)
    {
        var command = new WithdrawnMessageTaskCommand(id);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("{id}/Resend", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task ResendAsync(IEventBus eventBus, Guid id)
    {
        var command = new ResendMessageTaskCommand(id);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern(HttpMethod = "Post")]
    public async Task BindClientIdAsync([FromServices] IMasaConfiguration configuration, [FromServices] IAuthClient authClient, BindClientIdInputDto inputDto)
    {
        var systemId = $"{MasaStackConsts.MC_SYSTEM_ID}:{inputDto.ChannelCode}";
        var userSystemData = await authClient.UserService.GetSystemDataAsync<UserSystemDataDto>(systemId) ?? new();
        userSystemData.ClientId = inputDto.ClientId;
        await authClient.UserService.UpsertSystemDataAsync<UserSystemDataDto>(systemId, userSystemData);
    }

    [RoutePattern(HttpMethod = "Post")]
    public async Task HandleJobStatusNotifyAsync(IEventBus eventBus, [FromQuery] Guid jobId, [FromQuery] JobNotifyStatus status)
    {
        var command = new HandleJobStatusNotifyCommand(jobId, status);
        await eventBus.PublishAsync(command);
    }
}