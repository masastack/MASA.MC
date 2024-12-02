// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTasks.Jobs;

public class SendSimpleMessageJob : BackgroundJobBase<SendSimpleMessageArgs>
{
    private readonly IEventBus _eventBus;

    public SendSimpleMessageJob(ILogger<BackgroundJobBase<SendSimpleMessageArgs>>? logger
        , IEventBus eventBus) : base(logger)
    {
        _eventBus = eventBus;
    }

    protected override async Task ExecutingAsync(SendSimpleMessageArgs args)
    {
        var simpleInput = args.Adapt<SendSimpleTemplateMessageInputDto>();
        await _eventBus.PublishAsync(new SendSimpleTemplateMessageCommand(simpleInput));
    }
}