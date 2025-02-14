// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public SmsTemplateService(IServiceCollection services) : base("api/sms-template")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
        MapGet(GetListByChannelIdAsync);
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    [Topic(DAPR_PUBSUB_NAME, nameof(SmsTemplateSynchroIntegrationDomainEvent))]
    public async Task SyncAsync(IEventBus eventbus, SmsTemplateSynchroIntegrationDomainEvent @event)
    {
        var command = new SyncSmsTemplateCommand(@event.ChannelId);
        await eventbus.PublishAsync(command); 
    }
}