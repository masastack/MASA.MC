// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class ResendMessageTaskJob : BackgroundJobBase<ResendMessageTaskJobArgs>
{
    private readonly IMessageRecordRepository _messageRecordRepository;
    private readonly IMessageTaskHistoryRepository _messageTaskHistoryRepository;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public ResendMessageTaskJob(ILogger<BackgroundJobBase<ResendMessageTaskJobArgs>>? logger
        , IMessageRecordRepository messageRecordRepository
        , IMessageTaskHistoryRepository messageTaskHistoryRepository
        , IEventBus eventBus
        , IUnitOfWork unitOfWork) : base(logger)
    {
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
        _messageRecordRepository = messageRecordRepository;
        _messageTaskHistoryRepository = messageTaskHistoryRepository;
    }

    protected override async Task ExecutingAsync(ResendMessageTaskJobArgs args)
    {
        var records = await (await _messageRecordRepository.WithDetailsAsync()).Where(x => x.MessageTaskId == args.MessageTaskId && x.Success == false).ToListAsync();

        foreach (var item in records)
        {
            var eto = item.Channel.Type.GetRetryMessageEvent(item.Id);
            await _eventBus.PublishAsync(eto);
        }

        await _unitOfWork.SaveChangesAsync();

        var historys = await _messageTaskHistoryRepository.GetListAsync(x => x.MessageTaskId == args.MessageTaskId);
        foreach (var item in historys)
        {
            await _eventBus.PublishAsync(new UpdateMessageTaskHistoryStatusEvent(item.Id));
        }

        await _unitOfWork.SaveChangesAsync();

        await _eventBus.PublishAsync(new UpdateMessageTaskStatusEvent(args.MessageTaskId));
    }
}

