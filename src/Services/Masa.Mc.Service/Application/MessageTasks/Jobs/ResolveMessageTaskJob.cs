// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static Google.Api.ResourceDescriptor.Types;

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResolveMessageTaskJob : BackgroundJobBase<ResolveMessageTaskJobArgs>
{
    private readonly IChannelUserFinder _channelUserFinder;
    private readonly IMessageTaskRepository _messageTaskRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IDomainEventBus _eventBus;
    private readonly IUserContext _userContext;
    private readonly IMessageTaskJobService _messageTaskJobService;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveMessageTaskJob(ILogger<BackgroundJobBase<ResolveMessageTaskJobArgs>>? logger
        , IChannelUserFinder channelUserFinder
        , IMessageTaskRepository messageTaskRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IDomainEventBus eventBus
        , IUserContext userContext
        , IMessageTaskJobService messageTaskJobService
        , IUnitOfWork unitOfWork) : base(logger)
    {
        _channelUserFinder = channelUserFinder;
        _messageTaskRepository = messageTaskRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
        _eventBus = eventBus;
        _userContext = userContext;
        _messageTaskJobService = messageTaskJobService;
        _unitOfWork = unitOfWork;
    }

    protected override async Task ExecutingAsync(ResolveMessageTaskJobArgs args)
    {
        var messageTask = (await _messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == args.MessageTaskId);

        if (messageTask == null || messageTask.ReceiverType == ReceiverTypes.Broadcast)
            return;

        var receiverUsers = await _channelUserFinder.GetReceiverUsersAsync(messageTask.Channel, messageTask.Variables, messageTask.Receivers);
        messageTask.SetReceiverUsers(receiverUsers.ToList());
        var sendTime = DateTimeOffset.Now;

        await _messageTaskHistoryRepository.RemoveAsync(x => x.MessageTaskId == args.MessageTaskId);

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
                    var historyReceiverUsers = messageTask.GetHistoryReceiverUsers(i, sendingCount);
                    var history = new MessageTaskHistory(messageTask.Id, historyReceiverUsers, false, sendTime);
                    await _messageTaskHistoryRepository.AddAsync(history);
                }
            }
        }
        else
        {
            var history = new MessageTaskHistory(messageTask.Id, messageTask.ReceiverUsers, false, sendTime);
            history.ExecuteTask();
            await _messageTaskRepository.UpdateAsync(messageTask);
            await _messageTaskHistoryRepository.AddAsync(history);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return;
        }

        var userId = _userContext.GetUserId<Guid>();
        var operatorId = userId == default ? args.OperatorId : userId;

        var jobId = await _messageTaskJobService.RegisterJobAsync(messageTask.SchedulerJobId, args.MessageTaskId, messageTask.SendRules.CronExpression, operatorId, messageTask.DisplayName);

        messageTask.SetJobId(jobId);

        await _messageTaskRepository.UpdateAsync(messageTask);
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        if (string.IsNullOrEmpty(messageTask.SendRules.CronExpression) && jobId != default)
        {
            await _messageTaskJobService.StartJobAsync(jobId, operatorId);
        }
    }
}
