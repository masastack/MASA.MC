// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Services;

public class SmsTemplateDomainService : DomainService
{
    private readonly IMessageTemplateRepository _repository;

    public SmsTemplateDomainService(IDomainEventBus eventBus, IMessageTemplateRepository repository) : base(eventBus)
    {
        _repository = repository;
    }

    public async Task SyncAsync(Guid channelId)
    {
        await EventBus.PublishAsync(new SmsTemplateSyncDomainEvent(channelId));
    }
}
