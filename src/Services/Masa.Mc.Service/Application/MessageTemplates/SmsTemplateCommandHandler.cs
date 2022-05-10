// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class SmsTemplateCommandHandler
{
    private readonly ISmsTemplateRepository _repository;
    private readonly IIntegrationEventBus _integrationEventBus;
    private readonly SmsTemplateDomainService _domainService;

    public SmsTemplateCommandHandler(ISmsTemplateRepository repository, IIntegrationEventBus integrationEventBus, SmsTemplateDomainService domainService)
    {
        _repository = repository;
        _integrationEventBus = integrationEventBus;
        _domainService = domainService;
    }

    [EventHandler]
    public async Task SyncAsync(SyncSmsTemplateCommand command)
    {
        await _domainService.SyncAsync(command.ChannelId);
    }
}
