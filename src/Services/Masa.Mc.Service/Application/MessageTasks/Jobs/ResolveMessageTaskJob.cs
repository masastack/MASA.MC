// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResolveMessageTaskJob : BackgroundJobBase<ResolveMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public static ActivitySource ActivitySource { get; private set; } = new("Masa.Mc.Background");

    public ResolveMessageTaskJob(ILogger<BackgroundJobBase<ResolveMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ResolveMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var (channelUserFinder, messageTaskRepository, messageTaskHistoryRepository, messageTaskJobService, unitOfWork, userContext) = await GetRequiredServiceAsync(scope.ServiceProvider, args.Environment);

        var activity = string.IsNullOrEmpty(args.TraceParent) ? default : ActivitySource.StartActivity("", ActivityKind.Consumer, args.TraceParent);

        try
        {
            var messageTask = (await messageTaskRepository.WithDetailsAsync()).FirstOrDefault(x => x.Id == args.MessageTaskId);
            if (messageTask == null)
                return;

            var receiverUsers = await GetReceiverUsersAsync(channelUserFinder, messageTask);

            await messageTaskHistoryRepository.RemoveAsync(x => x.MessageTaskId == args.MessageTaskId);

            if (!messageTask.SendRules.IsCustom)
            {
                await GenerateSingleTaskHistoryAsync(messageTaskRepository, messageTaskHistoryRepository, messageTask, receiverUsers);
                await unitOfWork.SaveChangesAsync();
                await unitOfWork.CommitAsync();
                return;
            }

            await GenerateTaskHistoryAsync(messageTaskHistoryRepository, messageTask, receiverUsers);

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
        finally
        {
            activity?.Dispose();
        }
    }

    private async Task<List<MessageReceiverUser>> GetReceiverUsersAsync(IChannelUserFinder channelUserFinder, MessageTask messageTask)
    {
        var receiverUsers = new List<MessageReceiverUser>();

        if (messageTask.ReceiverType != ReceiverTypes.Broadcast)
        {
            receiverUsers = (await channelUserFinder.GetReceiverUsersAsync(messageTask.Channel, messageTask.Variables, messageTask.Receivers)).ToList();
        }

        return receiverUsers;
    }

    private async Task GenerateSingleTaskHistoryAsync(IMessageTaskRepository messageTaskRepository, IMessageTaskHistoryRepository messageTaskHistoryRepository, MessageTask messageTask, List<MessageReceiverUser> receiverUsers)
    {
        var sendTime = DateTimeOffset.Now;
        var history = new MessageTaskHistory(messageTask.Id, receiverUsers, false, sendTime);
        history.ExecuteTask();
        await messageTaskRepository.UpdateAsync(messageTask);
        await messageTaskHistoryRepository.AddAsync(history);
    }

    private async Task GenerateTaskHistoryAsync(IMessageTaskHistoryRepository messageTaskHistoryRepository, MessageTask messageTask, List<MessageReceiverUser> receiverUsers)
    {
        var sendTime = DateTimeOffset.Now;
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
    }

    private async Task<(IChannelUserFinder, IMessageTaskRepository, IMessageTaskHistoryRepository, IMessageTaskJobService, IUnitOfWork, IUserContext)> GetRequiredServiceAsync(IServiceProvider serviceProvider, string environment)
    {
        var multiEnvironmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(environment);
        var channelUserFinder = serviceProvider.GetRequiredService<IChannelUserFinder>();
        var messageTaskRepository = serviceProvider.GetRequiredService<IMessageTaskRepository>();
        var messageTaskHistoryRepository = serviceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var messageTaskJobService = serviceProvider.GetRequiredService<IMessageTaskJobService>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var userContext = serviceProvider.GetRequiredService<IUserContext>();

        return (channelUserFinder, messageTaskRepository, messageTaskHistoryRepository, messageTaskJobService, unitOfWork, userContext);
    }
}
