// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly IMessageTaskJobService _messageTaskJobService;
    private readonly IUserContext _userContext;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly II18n<DefaultResource> _i18n;
    private readonly IMultiEnvironmentContext _multiEnvironmentContext;
    private readonly MessageTemplateDomainService _messageTemplateDomainService;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IEventBus eventBus
        , IMessageTaskJobService messageTaskJobService
        , IUserContext userContext
        , IChannelRepository channelRepository
        , IMessageTemplateRepository messageTemplateRepository
        , IUnitOfWork unitOfWork
        , II18n<DefaultResource> i18n
        , IMultiEnvironmentContext multiEnvironmentContext
        , MessageTemplateDomainService messageTemplateDomainService)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _messageTaskJobService = messageTaskJobService;
        _userContext = userContext;
        _channelRepository = channelRepository;
        _messageTemplateRepository = messageTemplateRepository;
        _unitOfWork = unitOfWork;
        _i18n = i18n;
        _multiEnvironmentContext = multiEnvironmentContext;
        _messageTemplateDomainService = messageTemplateDomainService;
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageTaskCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        if (entity.IsEnabled)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ENABLED_STATUS_CANNOT_BE_DELETED);
        await _repository.RemoveAsync(entity);
    }

    [EventHandler]
    public async Task SendTestAsync(SendTestMessageTaskCommand command)
    {
        var inputDto = command.inputDto;
        var entity = await _repository.FindAsync(x => x.Id == inputDto.Id);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        if (!entity.ChannelId.HasValue)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_REQUIRED);
        if (entity.Channel.Type == ChannelType.Sms && string.IsNullOrEmpty(entity.Sign))
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.SIGN_REQUIRED);

        await CheckDataIsExistsAsync(entity);

        var receiverUsers = inputDto.ReceiverUsers.Adapt<List<MessageReceiverUser>>();
        var history = new MessageTaskHistory(entity.Id, receiverUsers, true);
        await _messageTaskHistoryRepository.AddAsync(history);
        await _unitOfWork.SaveChangesAsync();
        await _eventBus.PublishAsync(new ExecuteMessageTaskEvent(entity.Id, true));
    }

    [EventHandler]
    public async Task EnabledAsync(EnabledMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        entity.SetEnabled();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerJobId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _messageTaskJobService.EnableJobAsync(entity.SchedulerJobId, userId);
        }
    }

    [EventHandler]
    public async Task DisableAsync(DisableMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        if (await _messageTaskHistoryRepository.AnyAsync(x => x.MessageTaskId == command.MessageTaskId && x.Status == MessageTaskHistoryStatuses.Sending))
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TASK_DISABLE_HAS_HISTORY);
        entity.SetDisable();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerJobId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _messageTaskJobService.DisableJobAsync(entity.SchedulerJobId, userId);
        }
    }

    [EventHandler]
    public async Task SendOrdinaryMessageByInternalAsync(SendOrdinaryMessageByInternalCommand command)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == command.inputDto.ChannelCode);
        MasaArgumentException.ThrowIfNull(channel, _i18n.T("Channel"));

        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(ordinaryCommand);
    }

    [EventHandler]
    public async Task SendTemplateMessageByInternalAsync(SendTemplateMessageByInternalCommand command)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == command.inputDto.ChannelCode);
        MasaArgumentException.ThrowIfNull(channel, _i18n.T("Channel"));

        var template = await _messageTemplateRepository.FindAsync(x => x.Code == command.inputDto.TemplateCode);
        MasaArgumentException.ThrowIfNull(template, _i18n.T("MessageTemplate"));

        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        taskUpsertDto.EntityId = template.Id;
        var templateCommand = new CreateTemplateMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(templateCommand);
    }

    [EventHandler]
    public async Task SendOrdinaryMessageByExternalAsync(SendOrdinaryMessageByExternalCommand command)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == command.inputDto.ChannelCode);
        MasaArgumentException.ThrowIfNull(channel, _i18n.T("Channel"));

        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(ordinaryCommand);
    }

    [EventHandler]
    public async Task SendTemplateMessageByExternalAsync(SendTemplateMessageByExternalCommand command)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == command.inputDto.ChannelCode);
        MasaArgumentException.ThrowIfNull(channel, _i18n.T("Channel"));

        var template = await _messageTemplateRepository.FindAsync(x => x.Code == command.inputDto.TemplateCode);
        MasaArgumentException.ThrowIfNull(template, _i18n.T("MessageTemplate"));

        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        taskUpsertDto.EntityId = template.Id;
        var templateCommand = new CreateTemplateMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(templateCommand);
    }

    [EventHandler]
    public async Task WithdrawnAsync(WithdrawnMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.MessageTaskId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageTask"));

        entity.SetResult(MessageTaskStatuses.Cancel);
        await _repository.UpdateAsync(entity);

        var list = await _messageTaskHistoryRepository.GetListAsync(x => x.MessageTaskId == command.MessageTaskId);

        foreach (var item in list)
        {
            if (item.Status == MessageTaskHistoryStatuses.Withdrawn) continue;

            item.SetWithdraw();
            await _messageTaskHistoryRepository.UpdateAsync(item);
        }

        if (entity.SchedulerJobId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _messageTaskJobService.DisableJobAsync(entity.SchedulerJobId, userId);
        }
    }

    [EventHandler]
    public async Task ResendAsync(ResendMessageTaskCommand command)
    {
        var args = new ResendMessageTaskJobArgs()
        {
            MessageTaskId = command.MessageTaskId,
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            TraceParent = Activity.Current?.Id
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }

    [EventHandler]
    public async Task SendSimpleMessageAsync(SendSimpleTemplateMessageCommand command)
    {
        var template = await _messageTemplateRepository.AsNoTracking().FirstOrDefaultAsync(x => x.Code == command.InputDto.TemplateCode);
        MasaArgumentException.ThrowIfNull(template, _i18n.T("MessageTemplate"));

        var messageData = new MessageData(template.MessageContent, MessageEntityTypes.Template);
        messageData.RenderContent(command.InputDto.Variables);
        messageData.SetDataValue(nameof(MessageTemplate.TemplateId), template.TemplateId);
        messageData.SetDataValue(nameof(MessageTemplate.DisplayName), template.DisplayName);
        messageData.SetDataValue(nameof(MessageTemplate.Sign), template.Sign);
        messageData.SetDataValue(BusinessConsts.MESSAGE_TYPE, template.TemplateType.ToString());
        messageData.SetDataValue(nameof(MessageTemplate.Id), template.Id.ToString());

        var variables = _messageTemplateDomainService.ConvertVariables(template, command.InputDto.Variables);
        var channelType = Enumeration.FromValue<ChannelType>((int)command.InputDto.ChannelType);

        var eto = channelType.GetSendSimpleMessageEvent(command.InputDto.ChannelUserIdentity, command.InputDto.ChannelCode, messageData, variables, command.InputDto.Variables, command.InputDto.SystemId);
        await _eventBus.PublishAsync(eto);
    }

    private async Task CheckDataIsExistsAsync(MessageTask entity)
    {
        var channel = await _channelRepository.AsNoTracking().FirstOrDefaultAsync(e => e.Id == entity.ChannelId);
        if (channel == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.CHANNEL_REQUIRED);
        if (entity.EntityType == MessageEntityTypes.Template)
        {
            var messageTemplate = await _messageTemplateRepository.FindAsync(e => e.Id == entity.EntityId);
            if (messageTemplate == null)
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_TEMPLATE_NOT_EXIST);
        }
    }
}