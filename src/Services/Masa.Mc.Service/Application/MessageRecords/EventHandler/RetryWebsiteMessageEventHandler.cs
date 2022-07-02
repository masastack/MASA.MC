// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageRecords.EventHandler;

public class RetryWebsiteMessageEventHandler
{
    private readonly IHubContext<NotificationsHub> _hubContext;
    private readonly IMessageRecordRepository _messageRecordRepository;

    public RetryWebsiteMessageEventHandler(IHubContext<NotificationsHub> hubContext
        , IMessageRecordRepository messageRecordRepository)
    {
        _hubContext = hubContext;
        _messageRecordRepository = messageRecordRepository;
    }

    [EventHandler(1)]
    public async Task HandleEventAsync(RetryWebsiteMessageEvent eto)
    {
        var messageRecord = await _messageRecordRepository.FindAsync(x => x.Id == eto.MessageRecordId);
        if (messageRecord == null)
        {
            return;
        }

        try
        {
            var onlineClients = _hubContext.Clients.Users(messageRecord.UserId.ToString());
            await onlineClients.SendAsync(SignalRMethodConsts.GET_NOTIFICATION);
            messageRecord.SetResult(true, string.Empty);
        }
        catch (Exception ex)
        {
            messageRecord.SetResult(false, ex.Message);
            throw new UserFriendlyException("Resend message failed");
        }
        await _messageRecordRepository.UpdateAsync(messageRecord);
    }
}
