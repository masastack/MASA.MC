// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    private const string DAPR_PUBSUB_NAME = "pubsub";
    public SmsTemplateService(IServiceCollection services) : base(services, "api/sms-template")
    {
        MapGet(GetListByChannelIdAsync);
        MapPost(SyncAsync);
        MapPost(SyncHandlerAsync);
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task SyncAsync(IEventBus eventbus, SmsTemplateSyncInputDto inputDto)
    {
        var command = new SyncSmsTemplateCommand(inputDto.ChannelId);
        await eventbus.PublishAsync(command); 
    }

    [Topic(DAPR_PUBSUB_NAME, nameof(SmsTemplateSyncDomainEvent))]
    public async Task SyncHandlerAsync(SmsTemplateSyncDomainEvent @event, [FromServices] SmsTemplateSyncEventHandler handler)
    {
        await handler.HandleEvent(new SmsTemplateSyncDomainEvent(@event.ChannelId));
    }
}