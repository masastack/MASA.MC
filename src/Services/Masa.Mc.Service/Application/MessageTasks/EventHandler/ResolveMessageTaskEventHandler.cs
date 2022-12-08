// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.EventHandler;

public class ResolveMessageTaskEventHandler
{
    private readonly IChannelUserFinder _channelUserFinder;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IUserContext _userContext;
    private readonly IMessageTaskJobService _messageTaskJobService;

    public ResolveMessageTaskEventHandler(IChannelUserFinder channelUserFinder
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IDomainEventBus eventBus
        , IUserContext userContext
        , IMessageTaskJobService messageTaskJobService)
    {
        _channelUserFinder = channelUserFinder;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _userContext = userContext;
        _messageTaskJobService = messageTaskJobService;
    }

    [EventHandler(1)]
    public async Task QueryMessageTask(ResolveMessageTaskEvent eto)
    {
        var messageTask = (await _messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == eto.MessageTaskId);
        eto.MessageTask = messageTask;

        if (messageTask == null || messageTask.ReceiverType == ReceiverTypes.Broadcast)
        {
            eto.IsStop = true;
            return;
        }
    }

    [EventHandler(2)]
    public async Task QueryReceiverUsers(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var messageTask = eto.MessageTask;
        var receiverUsers = await _channelUserFinder.GetReceiverUsersAsync(messageTask.Channel.Type, messageTask.Variables, messageTask.Receivers);
        messageTask.ReceiverUsers.AddRange(receiverUsers);
    }

    [EventHandler(3)]
    public async Task CreateMessageTaskHistoryAsync(ResolveMessageTaskEvent eto)
    {
        var sendTime = DateTimeOffset.Now;
        var messageTask = eto.MessageTask;
        if (messageTask.SendRules.IsCustom)
        {
            var historyNum = messageTask.GetHistoryCount();
            var sendingCount = messageTask.GetSendingCount();

            var cronExpression = new CronExpression(messageTask.SendRules.CronExpression);
            cronExpression.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

            for (int i = 0; i < historyNum; i++)
            {
                var nextExcuteTime = cronExpression.GetNextValidTimeAfter(sendTime);
                if (nextExcuteTime.HasValue)
                {
                    sendTime = nextExcuteTime.Value;
                    var receiverUsers = messageTask.GetHistoryReceiverUsers(i, sendingCount);
                    var history = new MessageTaskHistory(messageTask.Id, receiverUsers, false, sendTime);
                    await _messageTaskHistoryRepository.AddAsync(history);
                }
            }
        }
        else
        {
            var history = new MessageTaskHistory(messageTask.Id, messageTask.ReceiverUsers, false, sendTime);
            history.ExecuteTask();
            await _messageTaskHistoryRepository.AddAsync(history);
        }

        if (!messageTask.SendRules.IsCustom)
        {
            eto.IsStop = true;
        }
    }

    [EventHandler(4)]
    public async Task AddSchedulerJobAsync(ResolveMessageTaskEvent eto)
    {
        if (eto.IsStop) return;

        var cronExpression = eto.MessageTask.SendRules.CronExpression;
        var userId = _userContext.GetUserId<Guid>();
        var operatorId = userId == default ? eto.OperatorId : userId;

        var jobId = await _messageTaskJobService.RegisterJobAsync(eto.MessageTaskId, cronExpression, operatorId, eto.MessageTask.DisplayName);
        eto.MessageTask.SetJobId(jobId);

        await _messageTaskRepository.UpdateAsync(eto.MessageTask);
        await _messageTaskHistoryRepository.UnitOfWork.SaveChangesAsync();

        if (string.IsNullOrEmpty(cronExpression) && jobId != default)
        {
            await _messageTaskJobService.StartJobAsync(jobId, operatorId);
        }
    }
}
