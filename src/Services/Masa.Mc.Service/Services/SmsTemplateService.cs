// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public SmsTemplateService(IServiceCollection services) : base("api/sms-template")
    {

    }

    [RoutePattern("GetListByChannelId", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    [Topic(DAPR_PUBSUB_NAME, nameof(SmsTemplateSynchroIntegrationDomainEvent))]
    [RoutePattern("Sync", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task SyncAsync(IEventBus eventbus, SmsTemplateSynchroIntegrationDomainEvent @event)
    {
        var command = new SyncSmsTemplateCommand(@event.ChannelId);
        await eventbus.PublishAsync(command); 
    }
}