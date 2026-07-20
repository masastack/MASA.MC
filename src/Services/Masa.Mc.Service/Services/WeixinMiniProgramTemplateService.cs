// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Services;

public class WeixinMiniProgramTemplateService : ServiceBase
{
    public WeixinMiniProgramTemplateService(IServiceCollection services) : base("api/weixin-mini-program-template")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    [RoutePattern("channels/{channelId}/templates", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<List<WeixinMiniProgramTemplateDto>> GetListAsync(
        IEventBus eventBus,
        [FromRoute]
        Guid channelId)
    {
        var query = new GetWeixinMiniProgramTemplateListQuery(channelId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
