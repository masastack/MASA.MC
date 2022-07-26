﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks;

public class MessageTaskCommandHandler
{
    private readonly IMessageTaskRepository _repository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly ISchedulerClient _schedulerClient;
    private readonly IUserContext _userContext;
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTemplateRepository _messageTemplateRepository;

    public MessageTaskCommandHandler(IMessageTaskRepository repository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IEventBus eventBus
        , ISchedulerClient schedulerClient
        , IUserContext userContext
        , IChannelRepository channelRepository
        , IMessageTemplateRepository messageTemplateRepository)
    {
        _repository = repository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _schedulerClient = schedulerClient;
        _userContext = userContext;
        _channelRepository = channelRepository;
        _messageTemplateRepository = messageTemplateRepository;
    }

    [EventHandler]
    public async Task DeleteAsync(DeleteMessageTaskCommand createCommand)
    {
        var entity = await _repository.FindAsync(x => x.Id == createCommand.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (entity.IsEnabled)
            throw new UserFriendlyException("enabled status cannot be deleted");
        await _repository.RemoveAsync(entity);
    }

    [EventHandler]
    public async Task SendTestAsync(SendTestMessageTaskCommand command)
    {
        var inputDto = command.inputDto;
        var entity = await _repository.FindAsync(x => x.Id == inputDto.Id);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (!entity.ChannelId.HasValue)
            throw new UserFriendlyException("please select the configuration channel");
        if (entity.Channel.Type == ChannelTypes.Sms && string.IsNullOrEmpty(entity.Sign))
            throw new UserFriendlyException("please fill in the signature of the task first");
        if (entity.Variables.Any(x => string.IsNullOrEmpty(x.Value.ToString())))
            throw new UserFriendlyException("please fill in the signature template variable of the task first");
        var receiverUsers = inputDto.ReceiverUsers.Adapt<List<MessageReceiverUser>>();
        var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
        var history = new MessageTaskHistory(entity.Id, taskHistoryNo, receiverUsers, true);
        await _messageTaskHistoryRepository.AddAsync(history);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();
        await _eventBus.PublishAsync(new ExecuteMessageTaskEvent(entity.Id, true));
    }

    [EventHandler]
    public async Task EnabledAsync(EnabledMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        entity.SetEnabled();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerJobId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _schedulerClient.SchedulerJobService.EnableAsync(new BaseSchedulerJobRequest { JobId = entity.SchedulerJobId, OperatorId = userId });
        }
    }

    [EventHandler]
    public async Task DisableAsync(DisableMessageTaskCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageTaskId);
        if (entity == null)
            throw new UserFriendlyException("messageTask not found");
        if (await _messageTaskHistoryRepository.AnyAsync(x => x.MessageTaskId == command.Input.MessageTaskId && x.Status == MessageTaskHistoryStatuses.Sending))
            throw new UserFriendlyException("the task has a sending task history and cannot be disabled.");
        entity.SetDisable();
        await _repository.UpdateAsync(entity);

        if (entity.SchedulerJobId != default)
        {
            var userId = _userContext.GetUserId<Guid>();
            await _schedulerClient.SchedulerJobService.DisableAsync(new BaseSchedulerJobRequest { JobId = entity.SchedulerJobId, OperatorId = userId });
        }
    }

    [EventHandler]
    public async Task SendOrdinaryMessageAsync(SendOrdinaryMessageTaskCommand command)
    {
        var channel = await _channelRepository.FindAsync(x=>x.Code== command.inputDto.ChannelCode);
        if (channel == null)
            throw new UserFriendlyException("Channel code not found");
        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        var ordinaryCommand = new CreateOrdinaryMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(ordinaryCommand);
    }

    [EventHandler]
    public async Task SendTemplateMessageAsync(SendTemplateMessageTaskCommand command)
    {
        var channel = await _channelRepository.FindAsync(x => x.Code == command.inputDto.ChannelCode);
        if (channel == null)
            throw new UserFriendlyException("Channel code not found");
        var template = await _messageTemplateRepository.FindAsync(x => x.Code == command.inputDto.TemplateCode);
        if (template == null)
            throw new UserFriendlyException("Template code not found");
        var taskUpsertDto = (MessageTaskUpsertDto)command.inputDto;
        taskUpsertDto.ChannelId = channel.Id;
        taskUpsertDto.EntityId = template.Id;
        var templateCommand = new CreateTemplateMessageTaskCommand(taskUpsertDto);
        await _eventBus.PublishAsync(templateCommand);
    }
}
