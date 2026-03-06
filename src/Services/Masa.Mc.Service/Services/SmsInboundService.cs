// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class SmsInboundService : ServiceBase
{
    public SmsInboundService() : base("api/sms-inbound")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    [RoutePattern("", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<PaginatedListDto<SmsInboundDto>> GetListAsync(IEventBus eventBus, [FromQuery] Guid channelId,
        [FromQuery] DateTimeOffset? startTime, [FromQuery] DateTimeOffset? endTime, [FromQuery] string mobile = "", 
        [FromQuery] string addSerial = "", [FromQuery] string smsContent = "",
        [FromQuery] string sorting = "", [FromQuery] int page = 1, [FromQuery] int pagesize = 10)
    {
        var input = new GetSmsInboundInputDto(channelId, mobile, addSerial, smsContent, startTime, endTime, sorting, page, pagesize);
        var query = new GetListSmsInboundQuery(input);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
