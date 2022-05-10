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

    public async Task SyncAsync(IEventBus eventbus, SmsTemplateSyncInputDto inputDto)
    {
        var command = new SyncSmsTemplateCommand(inputDto.ChannelId);
        await eventbus.PublishAsync(command); 
    }
}