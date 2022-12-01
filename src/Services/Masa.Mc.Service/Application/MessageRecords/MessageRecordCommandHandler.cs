// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class MessageRecordCommandHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IEventBus _eventBus;

    public MessageRecordCommandHandler(IMessageRecordRepository repository
        , IEventBus eventBus)
    {
        _repository = repository;
        _eventBus = eventBus;
    }

    [EventHandler]
    public async Task RetryAsync(RetryMessageRecordCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageRecordId);
        if (entity == null)
            throw new UserFriendlyException("MessageRecord not found");
        if (entity.Success == true)
            throw new UserFriendlyException("The message is successfully sent without resending");
        switch (entity.Channel.Type)
        {
            case ChannelTypes.Sms:
                await _eventBus.PublishAsync(new RetrySmsMessageEvent(entity.Id));
                break;
            case ChannelTypes.Email:
                await _eventBus.PublishAsync(new RetryEmailMessageEvent(entity.Id));
                break;
            case ChannelTypes.WebsiteMessage:
                await _eventBus.PublishAsync(new RetryWebsiteMessageEvent(entity.Id));
                break;
            default:
                break;
        }
    }
}
