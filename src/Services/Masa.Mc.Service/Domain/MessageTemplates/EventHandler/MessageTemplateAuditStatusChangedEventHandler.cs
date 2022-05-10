// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.EventHandler;

public class MessageTemplateAuditStatusChangedEventHandler
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateAuditStatusChangedEventHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToApproved(MessageTemplateAuditStatusChangedToApprovedDomainEvent @event)
    {
        var entity = await _repository.FindAsync(x => x.Id == @event.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatuses.Adopt, @event.Remarks);
        await _repository.UpdateAsync(entity);
    }

    [EventHandler]
    public async Task MessageTemplateStatusChangedToRefuse(MessageTemplateAuditStatusChangedToRefuseDomainEvent @event)
    {
        var entity = await _repository.FindAsync(x => x.Id == @event.TemplateId);
        entity.SetAuditStatus(MessageTemplateAuditStatuses.Fail, @event.Remarks);
        await _repository.UpdateAsync(entity);
    }
}