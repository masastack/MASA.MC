// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.ApiGateways.Caller.Services.Channels;

public class ChannelService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ChannelService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/channel";
    }
    public async Task<PaginatedListDto<ChannelDto>> GetListAsync(GetChannelInputDto inputDto)
    {
        return await GetAsync<GetChannelInputDto, PaginatedListDto<ChannelDto>>(string.Empty, inputDto) ?? new();
    }

    public async Task<ChannelDto?> GetAsync(Guid id)
    {
        return await GetAsync<ChannelDto>($"{id}");
    }

    public async Task CreateAsync(ChannelUpsertDto inputDto)
    {
        await PostAsync(string.Empty, inputDto);
    }

    public async Task UpdateAsync(Guid id, ChannelUpsertDto inputDto)
    {
        await PutAsync($"{id}", inputDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task<ChannelDto?> FindByCodeAsync(string code)
    {
        return await GetAsync<ChannelDto>($"FindByCode?code={code}");
    }

    public async Task<List<ChannelDto>> GetListByTypeAsync(ChannelTypes type)
    {
        return await GetAsync<List<ChannelDto>>($"GetListByType?type={type}") ?? new();
    }
}
