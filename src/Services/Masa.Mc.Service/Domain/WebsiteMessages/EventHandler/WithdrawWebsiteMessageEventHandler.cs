// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.EventHandler;

public class WithdrawWebsiteMessageEventHandler
{
    private readonly IWebsiteMessageRepository _repository;

    public WithdrawWebsiteMessageEventHandler(IWebsiteMessageRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(WithdrawWebsiteMessageEvent eto)
    {
        var messageRecords = await _repository.GetListAsync(x => x.MessageTaskHistoryId == eto.MessageTaskHistoryId);
        foreach (var item in messageRecords)
        {
            item.SetWithdraw();
        }
        await _repository.UpdateManyAsync(messageRecords, true);
    }
}
