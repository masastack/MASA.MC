// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.EventHandler;

public class WithdrawMessageRecordEventHandler
{
    private readonly IMessageRecordRepository _repository;

    public WithdrawMessageRecordEventHandler(IMessageRecordRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task HandleEventAsync(WithdrawMessageRecordEvent eto)
    {
        var messageRecords = await _repository.GetListAsync(x => x.MessageTaskHistoryId == eto.MessageTaskHistoryId);
        foreach (var item in messageRecords)
        {
            item.SetWithdraw();
        }
        await _repository.UpdateManyAsync(messageRecords, true);
    }
}
