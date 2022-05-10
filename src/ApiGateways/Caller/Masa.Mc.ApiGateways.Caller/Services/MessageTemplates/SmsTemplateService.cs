// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.MessageTemplates;

public class SmsTemplateService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal SmsTemplateService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sms-template";
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(Guid channelId)
    {
        return await GetAsync<List<SmsTemplateDto>>($"GetListByChannelId?channelId={channelId}") ?? new();
    }

    public async Task SyncAsync(SmsTemplateSyncInputDto inputDto)
    {
        await PostAsync("Sync", inputDto);
    }
}
