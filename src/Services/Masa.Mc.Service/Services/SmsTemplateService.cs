// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    public SmsTemplateService(IServiceCollection services) : base(services, "api/sms-template")
    {
        MapGet(GetListByChannelIdAsync);
        MapPost(SyncAsync);
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    [Topic(DaprConsts.DAPR_PUBSUB_NAME, nameof(SmsTemplateSynchroIntegrationDomainEvent))]
    public async Task SyncAsync(IEventBus eventbus, SmsTemplateSynchroIntegrationDomainEvent @event)
    {
        var command = new SyncSmsTemplateCommand(@event.ChannelId);
        await eventbus.PublishAsync(command); 
    }
}