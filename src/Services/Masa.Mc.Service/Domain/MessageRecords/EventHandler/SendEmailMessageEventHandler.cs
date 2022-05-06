// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class SendEmailMessageEventHandler
{
    private readonly IMessageRecordRepository _messageRecordRepository;

    public SendEmailMessageEventHandler(IMessageRecordRepository messageRecordRepository)
    {
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler]
    public async Task HandleEventAsync(SendEmailMessageEvent @event)
    {
        var messageRecords = await _messageRecordRepository.GetListAsync(x => x.MessageTaskHistoryId == @event.MessageTaskHistoryId);
    }
}
