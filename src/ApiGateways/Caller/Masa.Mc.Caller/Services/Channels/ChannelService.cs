﻿namespace Masa.Mc.Caller.Services.Channels;

public class ChannelService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ChannelService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/channel";
    }
    public async Task<PaginatedListDto<ChannelDto>> GetListAsync(GetChannelInput input)
    {
        var queryArguments = new Dictionary<string, string?>()
        {
            { "filter", input.Filter.ToString() },
            { "displayName", input.DisplayName.ToString() },
            { "type",input.Type?.ToString()},
            { "sorting", input.Sorting.ToString() },
            { "page", input.Page.ToString() },
            { "pageSize", input.PageSize.ToString() }
        };
        var url = QueryHelpers.AddQueryString(string.Empty, queryArguments);
        return await GetAsync<PaginatedListDto<ChannelDto>>(url) ?? new();
    }

    public async Task<ChannelDto?> GetAsync(Guid id)
    {
        return await GetAsync<ChannelDto>($"{id}");
    }

    public async Task CreateAsync(ChannelCreateUpdateDto input)
    {
        await PostAsync(string.Empty, input);
    }

    public async Task UpdateAsync(Guid id, ChannelCreateUpdateDto input)
    {
        await PutAsync($"{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task<ChannelDto?> FindByCodeAsync(string code)
    {
        return await GetAsync<ChannelDto>($"FindByCode?code={code}");
    }

    public async Task<List<ChannelDto>> GetListByTypeAsync(ChannelType type)
    {
        return await GetAsync<List<ChannelDto>>($"GetListByType?type={type}") ?? new();
    }
}
