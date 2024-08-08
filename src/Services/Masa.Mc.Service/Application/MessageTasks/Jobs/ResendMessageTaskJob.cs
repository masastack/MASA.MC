// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResendMessageTaskJob : BackgroundJobBase<ResendMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public static ActivitySource ActivitySource { get; private set; } = new("Masa.Mc.Background");

    public ResendMessageTaskJob(ILogger<BackgroundJobBase<ResendMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ResendMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var (messageRecordRepository, messageTaskHistoryRepository, eventBus, unitOfWork) = await GetRequiredServiceAsync(scope.ServiceProvider, args.Environment);

        var activity = string.IsNullOrEmpty(args.TraceParent) ? default : ActivitySource.StartActivity("", ActivityKind.Consumer, args.TraceParent);

        try
        {
            var records = await (await messageRecordRepository.WithDetailsAsync()).Where(x => x.MessageTaskId == args.MessageTaskId && x.Success == false).ToListAsync();

            foreach (var item in records)
            {
                var eto = item.Channel.Type.GetRetryMessageEvent(item.Id);
                await eventBus.PublishAsync(eto);
            }

            await unitOfWork.SaveChangesAsync();

            var historys = await messageTaskHistoryRepository.GetListAsync(x => x.MessageTaskId == args.MessageTaskId);
            foreach (var item in historys)
            {
                await eventBus.PublishAsync(new UpdateMessageTaskHistoryStatusEvent(item.Id));
            }

            await unitOfWork.SaveChangesAsync();

            await eventBus.PublishAsync(new UpdateMessageTaskStatusEvent(args.MessageTaskId));
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private async Task<(IMessageRecordRepository, IMessageTaskHistoryRepository, IEventBus, IUnitOfWork)> GetRequiredServiceAsync(IServiceProvider serviceProvider, string environment)
    {
        var multiEnvironmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(environment);
        var messageRecordRepository = serviceProvider.GetRequiredService<IMessageRecordRepository>();
        var messageTaskHistoryRepository = serviceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        return (messageRecordRepository, messageTaskHistoryRepository, eventBus, unitOfWork);
    }
}

