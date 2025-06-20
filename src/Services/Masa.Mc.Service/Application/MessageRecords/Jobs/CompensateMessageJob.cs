// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.Jobs;

public class CompensateMessageJob : BackgroundJobBase<CompensateMessageArgs>
{
    private readonly IEventBus _eventBus;
    private readonly IChannelRepository _repository;

    public CompensateMessageJob(ILogger<BackgroundJobBase<CompensateMessageArgs>>? logger, IEventBus eventBus, IChannelRepository repository) : base(logger)
    {
        _eventBus = eventBus;
        _repository = repository;
    }

    protected override async Task ExecutingAsync(CompensateMessageArgs args)
    {
        var channel = await _repository.FindAsync(x => x.Code == args.ChannelCode);

        if (channel == null)
        {
            throw new InvalidOperationException($"Channel with code {args.ChannelCode} not found.");
        }

        var input = new SendTemplateMessageByInternalInputDto
        {
            ChannelCode = args.ChannelCode,
            ChannelType = (ChannelTypes?)Enum.Parse(typeof(ChannelTypes), channel.Type.Id.ToString()),
            TemplateCode = args.TemplateCode,
            ReceiverType = ReceiverTypes.Assign,
            Receivers = new()
           {
               new InternalReceiverDto
               {
                   SubjectId = args.UserId,
                   Type = MessageTaskReceiverTypes.User
               }
           },
            Variables = args.Variables,
        };

        var command = new SendTemplateMessageByInternalCommand(input);
        await _eventBus.PublishAsync(command);
    }
}
