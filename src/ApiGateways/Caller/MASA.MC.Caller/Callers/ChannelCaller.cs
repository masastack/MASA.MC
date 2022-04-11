namespace Masa.Mc.Caller.Callers;

public class ChannelCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }
    private readonly string _prefix = "/api/channel";

    public ChannelCaller(IServiceProvider serviceProvider, IOptions<Settings> settings) : base(serviceProvider)
    {
        BaseAddress = settings.Value.McServiceBaseUrl;
        Name = nameof(ChannelCaller);
    }

    public async Task<PaginatedListDto<ChannelDto>> GetListAsync(GetChannelInput input)
    {
        var queryArguments = new Dictionary<string, string?>()
        {
            { "filter", input.Filter.ToString() },
            { "displayName", input.DisplayName.ToString() },
            { "page", input.Page.ToString() },
            { "pageSize", input.PageSize.ToString() }
        };
        if (input.Type.HasValue) queryArguments.Add("type", input.Type.ToString());
        var url = QueryHelpers.AddQueryString(_prefix, queryArguments);
        return await CallerProvider.GetAsync<PaginatedListDto<ChannelDto>>(url) ?? new();
    }

    public async Task<ChannelDto?> GetAsync(Guid id)
    {
        return await CallerProvider.GetAsync<ChannelDto> ($"{_prefix}/{id}");
    }

    public async Task CreateAsync(ChannelCreateUpdateDto input)
    {
        await CallerProvider.PostAsync(_prefix, input);
    }

    public async Task UpdateAsync(Guid id, ChannelCreateUpdateDto input)
    {
        await CallerProvider.PutAsync($"{_prefix}/{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await CallerProvider.DeleteAsync($"{_prefix}/{id}",null);
    }

    public async Task<ChannelDto?> FindByCodeAsync(string code)
    {
        return await CallerProvider.GetAsync<ChannelDto>($"{_prefix}/FindByCode?code={code}");
    }
}
