// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.WebsiteMessages;

public class WebsiteMessageService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public WebsiteMessageService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/website-message";
    }

    public async Task<PaginatedListDto<WebsiteMessageDto>> GetListAsync(GetWebsiteMessageInputDto inputDto)
    {
        return await GetAsync<GetWebsiteMessageInputDto, PaginatedListDto<WebsiteMessageDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<WebsiteMessageDto?> GetAsync(Guid id)
    {
        return await GetAsync<WebsiteMessageDto>($"{id}");
    }

    public async Task<List<WebsiteMessageChannelListDto>> GetChannelListAsync()
    {
        return await GetAsync<List<WebsiteMessageChannelListDto>>(nameof(GetChannelListAsync));
    }

    public async Task SetAllReadAsync(SetAllReadWebsiteMessageInputDto inputDto)
    {
        await PostAsync(nameof(SetAllReadAsync), inputDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task ReadAsync(ReadWebsiteMessageInputDto inputDto)
    {
        await PostAsync(nameof(ReadAsync), inputDto);
    }
}
