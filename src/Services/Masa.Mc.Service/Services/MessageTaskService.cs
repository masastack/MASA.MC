// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class MessageTaskService : ServiceBase
{
    IEventBus _eventBus => GetRequiredService<IEventBus>();

    public MessageTaskService(IServiceCollection services) : base("api/message-task")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        MapGet(GetListAsync, string.Empty);
        MapGet(GenerateReceiverImportTemplateAsync);
        MapGet(GetMessageTaskReceiverListAsync);
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync([FromQuery] Guid? channelId, [FromQuery] MessageEntityTypes? entityType, [FromQuery] bool? isDraft, [FromQuery] bool? isEnabled, [FromQuery] MessageTaskTimeTypes? timeType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime, [FromQuery] MessageTaskStatuses? status, [FromQuery] MessageTaskSources? source, [FromQuery] string systemId, [FromQuery] string channelCode = "", [FromQuery] string filter = "", [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var inputDto = new GetMessageTaskInputDto(filter, channelId, channelCode,entityType, isDraft, isEnabled, timeType, startTime, endTime, status, source, systemId, sorting, page, pagesize);
        var query = new GetMessageTaskListQuery(inputDto);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<MessageTaskDto> GetAsync(Guid id)
    {
        var query = new GetMessageTaskQuery(id);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync([FromBody] MessageTaskUpsertDto inputDto)
    {
        switch (inputDto.EntityType)
        {
            case MessageEntityTypes.Ordinary:
                var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(inputDto);
                await _eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityTypes.Template:
                var templateCommand = new CreateTemplateMessageTaskCommand(inputDto);
                await _eventBus.PublishAsync(templateCommand);
                break;
            default:
                throw new UserFriendlyException("unknown message task type");
        }

    }

    public async Task UpdateAsync(Guid id, [FromBody] MessageTaskUpsertDto inputDto)
    {
        switch (inputDto.EntityType)
        {
            case MessageEntityTypes.Ordinary:
                var ordinaryCommand = new UpdateOrdinaryMessageTaskCommand(id, inputDto);
                await _eventBus.PublishAsync(ordinaryCommand);
                break;
            case MessageEntityTypes.Template:
                var templateCommand = new UpdateTemplateMessageTaskCommand(id, inputDto);
                await _eventBus.PublishAsync(templateCommand);
                break;
            default:
                throw new UserFriendlyException("unknown message task type");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var command = new DeleteMessageTaskCommand(id);
        await _eventBus.PublishAsync(command);
    }

    public async Task SendAsync(SendMessageTaskInputDto inputDto)
    {
        var command = new SendMessageTaskCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    public async Task SendTestAsync(SendTestMessageTaskInputDto inputDto)
    {
        var command = new SendTestMessageTaskCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }


    [RoutePattern("{id}/enabled/{isEnabled}", StartWithBaseUri = true, HttpMethod = "Put")]
    public async Task SetIsEnabledAsync(Guid id, bool isEnabled)
    {
        if (isEnabled)
        {
            var command = new EnabledMessageTaskCommand(id);
            await _eventBus.PublishAsync(command);
        }
        else
        {
            var command = new DisableMessageTaskCommand(id);
            await _eventBus.PublishAsync(command);
        }
    }

    public async Task<byte[]> GenerateReceiverImportTemplateAsync(Guid? messageTemplatesId, ChannelTypes channelType)
    {
        var query = new GenerateReceiverImportTemplateQuery(messageTemplatesId, channelType);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<ImportResultDto<MessageTaskReceiverDto>> ImportReceiversAsync(ImportReceiversDto dto)
    {
        var command = new ImportReceiversCommand(dto);
        await _eventBus.PublishAsync(command);
        return command.Result;
    }

    public async Task<List<MessageTaskReceiverDto>> GetMessageTaskReceiverListAsync(IAuthClient authClient, [FromQuery] string filter = "")
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
        await _eventBus.PublishAsync(receiverGroupQuery);
        var receiverGroupList = receiverGroupQuery.Result.Result;
        list.AddRange(receiverGroupList.Select(x => new MessageTaskReceiverDto
        {
            SubjectId = x.Id,
            DisplayName = x.DisplayName,
            Type = MessageTaskReceiverTypes.Group
        }));

        return list;
    }

    public async Task<long> ResolveReceiversCountAsync(List<MessageTaskReceiverDto> dto)
    {
        var query = new ResolveReceiversCountQuery(dto);
        await _eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task ExecuteAsync(IMultiEnvironmentContext multiEnvironmentContext, Guid messageTaskId, Guid taskId)
    {
        var args = new ExecuteMessageTaskJobArgs()
        {
            MessageTaskId = messageTaskId,
            IsTest = false,
            JobId = default,
            TaskId = taskId,
            Environment = multiEnvironmentContext.CurrentEnvironment,
            TraceParent = Activity.Current?.Id
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }

    [AllowAnonymous]
    public async Task SendOrdinaryMessageByInternalAsync(SendOrdinaryMessageByInternalInputDto inputDto)
    {
        var command = new SendOrdinaryMessageByInternalCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    public async Task SendTemplateMessageByInternalAsync(SendTemplateMessageByInternalInputDto inputDto)
    {
        var command = new SendTemplateMessageByInternalCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    public async Task SendOrdinaryMessageByExternalAsync(SendOrdinaryMessageByExternalInputDto inputDto)
    {
        var command = new SendOrdinaryMessageByExternalCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    public async Task SendTemplateMessageByExternalAsync(SendTemplateMessageByExternalInputDto inputDto)
    {
        if (inputDto.ChannelType == ChannelTypes.Sms && inputDto.Receivers.Count == 1)
        {
            var args = inputDto.Adapt<SendSimpleMessageArgs>();
            await BackgroundJobManager.EnqueueAsync(args);
            return;
        }

        var command = new SendTemplateMessageByExternalCommand(inputDto);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("{id}/withdrawn", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task WithdrawnAsync(Guid id)
    {
        var command = new WithdrawnMessageTaskCommand(id);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern("{id}/resend", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task ResendAsync(Guid id)
    {
        var command = new ResendMessageTaskCommand(id);
        await _eventBus.PublishAsync(command);
    }

    [RoutePattern(HttpMethod = "Post")]
    public async Task BindClientIdAsync([FromServices] IMasaConfiguration configuration, [FromServices] IAuthClient authClient, BindClientIdInputDto inputDto)
    {
        if (inputDto.Platform.HasValue)
        {
            var query = new FindChannelByCodeQuery(inputDto.ChannelCode);
            await _eventBus.PublishAsync(query);
            var channel = query.Result;

            var command = new BindAppDeviceTokenCommand(channel.Id, inputDto.ClientId, inputDto.Platform.Value);
            await _eventBus.PublishAsync(command);
        }

        var systemId = $"{MasaStackProject.MC.Name}:{inputDto.ChannelCode}";
        var userSystemData = await authClient.UserService.GetSystemDataAsync<UserSystemDataDto>(systemId) ?? new();
        userSystemData.ClientId = inputDto.ClientId;
        await authClient.UserService.UpsertSystemDataAsync<UserSystemDataDto>(systemId, userSystemData);
    }

    [RoutePattern(HttpMethod = "Post")]
    public async Task HandleJobStatusNotifyAsync(IEventBus eventBus, [FromBody] HandleJobStatusNotifyInputDto inputDto)
    {
        var command = new HandleJobStatusNotifyCommand(inputDto.JobId, inputDto.Status);
        await eventBus.PublishAsync(command);
    }

    [AllowAnonymous]
    [RoutePattern("simple-send", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task SendSimpleMessageAsync(IEventBus eventBus, [FromBody] SendSimpleTemplateMessageInputDto inputDto)
    {
        var command = new SendSimpleTemplateMessageCommand(inputDto);
        await eventBus.PublishAsync(command);
    }
}