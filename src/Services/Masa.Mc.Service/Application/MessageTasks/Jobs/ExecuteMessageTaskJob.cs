// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ExecuteMessageTaskJob : BackgroundJobBase<ExecuteMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public ExecuteMessageTaskJob(ILogger<BackgroundJobBase<ExecuteMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ExecuteMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var (channelRepository, messageTaskRepository, messageTaskHistoryRepository, eventBus, domainService, messageTaskJobService, unitOfWork) = await GetRequiredServiceAsync(scope.ServiceProvider, args.Environment);

        var activity = string.IsNullOrEmpty(args.TraceParent) ? default : MessageTaskExecuteJobConsts.ActivitySource.StartActivity("", ActivityKind.Consumer, args.TraceParent);

        try
        {
            var history = await messageTaskHistoryRepository.FindWaitSendAsync(args.MessageTaskId, args.IsTest);

            if (history == null)
            {
                var messageTask = await messageTaskRepository.FindAsync(x => x.Id == args.MessageTaskId, false);
                if (messageTask == null) return;

                Guid userId = Guid.Empty;
                await messageTaskJobService.DisableJobAsync(messageTask.SchedulerJobId, userId);
                return;
            }
            history.SetTaskId(args.TaskId);
            var messageData = await domainService.GetMessageDataAsync(history.MessageTask, history.MessageTask.Variables);
            history.SetSending();

            if (!args.IsTest && !history.MessageTask.SendTime.HasValue)
            {
                history.MessageTask.SetSending();
            }

            await messageTaskHistoryRepository.UpdateAsync(history);

            var channel = await channelRepository.FindAsync(x => x.Id == history.MessageTask.ChannelId);

            var eto = channel.Type.GetSendMessageEvent(history.MessageTask.ChannelId.Value, messageData, history);
            await eventBus.PublishAsync(eto);
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private async Task<(IChannelRepository, IMessageTaskRepository, IMessageTaskHistoryRepository, IDomainEventBus, MessageTaskDomainService, IMessageTaskJobService, IUnitOfWork)> GetRequiredServiceAsync(IServiceProvider serviceProvider, string environment)
    {
        var multiEnvironmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(environment);
        var channelRepository = serviceProvider.GetRequiredService<IChannelRepository>();
        var messageTaskRepository = serviceProvider.GetRequiredService<IMessageTaskRepository>();
        var messageTaskHistoryRepository = serviceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var eventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        var domainService = serviceProvider.GetRequiredService<MessageTaskDomainService>();
        var messageTaskJobService = serviceProvider.GetRequiredService<IMessageTaskJobService>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        return (channelRepository, messageTaskRepository, messageTaskHistoryRepository, eventBus, domainService, messageTaskJobService, unitOfWork);
    }
}
