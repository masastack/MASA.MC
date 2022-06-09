// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTasks.EventHandler;

public class AddMessageTaskHistoryEventHandler
{
    private readonly IMessageTaskHistoryRepository _repository;
    private readonly MessageTaskHistoryDomainService _messageTaskHistoryDomainService;
    public AddMessageTaskHistoryEventHandler(IMessageTaskHistoryRepository repository
        , MessageTaskHistoryDomainService messageTaskHistoryDomainService)
    {
        _repository = repository;
        _messageTaskHistoryDomainService = messageTaskHistoryDomainService;
    }

    [EventHandler]
    public async Task HandleEventAsync(AddMessageTaskHistoryEvent eto)
    {
        var taskHistoryNo = $"SJ{UtilConvert.GetGuidToNumber()}";
        var history = new MessageTaskHistory(eto.MessageTask.Id, taskHistoryNo, eto.ReceiverType, eto.selectReceiverType, eto.Receivers, eto.SendRules, null, eto.Sign, eto.Variables);
        await _repository.AddAsync(history);
        await _repository.UnitOfWork.SaveChangesAsync();
        await _messageTaskHistoryDomainService.ExecuteAsync(history.Id);
    }
}
