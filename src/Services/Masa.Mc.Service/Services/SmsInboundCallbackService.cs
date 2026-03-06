// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsInboundCallbackService : ServiceBase
{
    IEventBus _eventBus => GetRequiredService<IEventBus>();

    public SmsInboundCallbackService() : base("api/sms-inbound/callback")
    {
    }

    [RoutePattern("yunmas/{channelId}", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<string> ReceiveYunMasInboundSmsAsync([FromBody] SmsInboundReceiveInput input, Guid channelId)
    {
        var command = new ReceiveSmsInboundCommand(input, SmsInboundProviders.YunMas, channelId);
        await _eventBus.PublishAsync(command);
        return "ok";
    }
}
