// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ExecuteMessageTaskJob : BackgroundJobBase<ExecuteMessageTaskJobArgs>
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly MessageTaskDomainService _domainService;
    private readonly IMessageTaskJobService _messageTaskJobService;
    private readonly IUnitOfWork _unitOfWork;

    public ExecuteMessageTaskJob(ILogger<BackgroundJobBase<ExecuteMessageTaskJobArgs>>? logger
        ,IChannelRepository channelRepository
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IDomainEventBus eventBus
        , MessageTaskDomainService domainService
        , IMessageTaskJobService messageTaskJobService
        , IUnitOfWork unitOfWork) : base(logger)
    {
        _channelRepository = channelRepository;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _domainService = domainService;
        _messageTaskJobService = messageTaskJobService;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecutingAsync(ExecuteMessageTaskJobArgs args)
    {
        var history = await _messageTaskHistoryRepository.FindWaitSendAsync(args.MessageTaskId, args.IsTest);

        if (history == null)
        {
            var messageTask = await _messageTaskRepository.FindAsync(x => x.Id == args.MessageTaskId, false);
            if (messageTask == null) return;

            Guid userId = Guid.Empty;
            await _messageTaskJobService.DisableJobAsync(messageTask.SchedulerJobId, userId);
            return;
        }
        history.SetTaskId(args.TaskId);
        var messageData = await _domainService.GetMessageDataAsync(history.MessageTask, history.MessageTask.Variables);
        history.SetSending();

        if (!args.IsTest && !history.MessageTask.SendTime.HasValue)
        {
            history.MessageTask.SetSending();
        }

        await _messageTaskHistoryRepository.UpdateAsync(history);
        await _unitOfWork.SaveChangesAsync();

        var channel = await _channelRepository.FindAsync(x => x.Id == history.MessageTask.ChannelId);

        var eto = channel.Type.GetSendMessageEvent(history.MessageTask.ChannelId.Value, messageData, history);
        await _eventBus.PublishAsync(eto);
    }
}
