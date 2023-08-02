// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResolveMessageTaskJob : BackgroundJobBase<ResolveMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;


    public ResolveMessageTaskJob(ILogger<BackgroundJobBase<ResolveMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ResolveMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(args.Environment);
        var channelUserFinder = scope.ServiceProvider.GetRequiredService<IChannelUserFinder>();
        var messageTaskRepository = scope.ServiceProvider.GetRequiredService<IMessageTaskRepository>();
        var messageTaskHistoryRepository = scope.ServiceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var messageTaskJobService = scope.ServiceProvider.GetRequiredService<IMessageTaskJobService>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();

        var sendTime = DateTimeOffset.Now;
        var messageTask = (await messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == args.MessageTaskId);
        if (messageTask == null)
            return;

        var receiverUsers = new List<MessageReceiverUser>();

        if (messageTask.ReceiverType != ReceiverTypes.Broadcast)
        {
            receiverUsers = (await channelUserFinder.GetReceiverUsersAsync(messageTask.Channel, messageTask.Variables, messageTask.Receivers)).ToList();
        }

        await messageTaskHistoryRepository.RemoveAsync(x => x.MessageTaskId == args.MessageTaskId);

        if (messageTask.ReceiverType == ReceiverTypes.Broadcast || !messageTask.SendRules.IsCustom)
        {
            var history = new MessageTaskHistory(messageTask.Id, receiverUsers, false, sendTime);
            history.ExecuteTask();
            await messageTaskRepository.UpdateAsync(messageTask);
            await messageTaskHistoryRepository.AddAsync(history);
            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitAsync();

            return;
        }

        var historyNum = messageTask.GetHistoryCount(receiverUsers);
        var sendingCount = messageTask.GetSendingCount(receiverUsers);

        var cronExpression = new CronExpression(messageTask.SendRules.CronExpression);
        cronExpression.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");

        for (int i = 0; i < historyNum; i++)
        {
            var nextExcuteTime = cronExpression.GetNextValidTimeAfter(sendTime);
            if (nextExcuteTime.HasValue)
            {
                sendTime = nextExcuteTime.Value;
                var historyReceiverUsers = messageTask.GetHistoryReceiverUsers(receiverUsers, i, sendingCount);
                var history = new MessageTaskHistory(messageTask.Id, historyReceiverUsers, false, sendTime);
                await messageTaskHistoryRepository.AddAsync(history);
            }
        }


        var userId = userContext.GetUserId<Guid>();
        var operatorId = userId == default ? args.OperatorId : userId;

        var jobId = await messageTaskJobService.RegisterJobAsync(messageTask.SchedulerJobId, args.MessageTaskId, messageTask.SendRules.CronExpression, operatorId, messageTask.DisplayName);
        messageTask.SetJobId(jobId);

        await messageTaskRepository.UpdateAsync(messageTask);
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();

        if (string.IsNullOrEmpty(messageTask.SendRules.CronExpression) && jobId != default)
        {
            await messageTaskJobService.StartJobAsync(jobId, operatorId);
        }
    }
}
