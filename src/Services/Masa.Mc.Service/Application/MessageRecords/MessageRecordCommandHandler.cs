// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords;

public class MessageRecordCommandHandler
{
    private readonly IMessageRecordRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly II18n<DefaultResource> _i18n;

    public MessageRecordCommandHandler(IMessageRecordRepository repository
        , IEventBus eventBus
        , II18n<DefaultResource> i18n)
    {
        _repository = repository;
        _eventBus = eventBus;
        _i18n = i18n;
    }

    [EventHandler]
    public async Task RetryAsync(RetryMessageRecordCommand command)
    {
        var entity = await _repository.FindAsync(x => x.Id == command.Input.MessageRecordId);
        MasaArgumentException.ThrowIfNull(entity, _i18n.T("MessageRecord"));

        if (entity.Success == true)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.MESSAGE_HAS_BEEN_SENT_SUCCESSFULLY_NO_NEED_TO_RESEND);

        var eto = entity.Channel.Type.GetRetryMessageEvent(entity.Id);
        await _eventBus.PublishAsync(eto);
    }
}
