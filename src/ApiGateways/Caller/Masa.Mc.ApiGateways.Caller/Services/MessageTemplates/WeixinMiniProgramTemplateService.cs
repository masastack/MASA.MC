// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageTemplates;

public class WeixinMiniProgramTemplateService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal WeixinMiniProgramTemplateService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/weixin-mini-program-template";
    }

    public async Task<List<WeixinMiniProgramTemplateDto>> GetListAsync(Guid channelId)
    {
        return await GetAsync<List<WeixinMiniProgramTemplateDto>>($"channels/{channelId}/templates") ?? new();
    }
}
