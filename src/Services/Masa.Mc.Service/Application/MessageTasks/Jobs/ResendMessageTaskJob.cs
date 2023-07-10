// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResendMessageTaskJob : BackgroundJobBase<ResendMessageTaskJobArgs>
{
    private readonly IServiceProvider _serviceProvider;

    public ResendMessageTaskJob(ILogger<BackgroundJobBase<ResendMessageTaskJobArgs>>? logger
        , IServiceProvider serviceProvider) : base(logger)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(ResendMessageTaskJobArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var multiEnvironmentSetter = scope.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(args.Environment);
        var messageRecordRepository = scope.ServiceProvider.GetRequiredService<IMessageRecordRepository>();
        var messageTaskHistoryRepository = scope.ServiceProvider.GetRequiredService<IMessageTaskHistoryRepository>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

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
}

