namespace Masa.Mc.Caller.Callers;

public class ReceiverGroupCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }
    private readonly string _prefix = "/api/receiver-group";

    public ReceiverGroupCaller(IServiceProvider serviceProvider, IOptions<Settings> settings) : base(serviceProvider)
    {
        BaseAddress = settings.Value.McServiceBaseUrl;
        Name = nameof(ReceiverGroupCaller);
    }

    public async Task<PaginatedListDto<ReceiverGroupDto>> GetListAsync(GetReceiverGroupInput input)
    {
        var queryArguments = new Dictionary<string, string?>()
        {
            { "filter", input.Filter.ToString() },
            { "page", input.Page.ToString() },
            { "pageSize", input.PageSize.ToString() }
        };
        var url = QueryHelpers.AddQueryString(_prefix, queryArguments);
        return await CallerProvider.GetAsync<PaginatedListDto<ReceiverGroupDto>>(url) ?? new();
    }

    public async Task<ReceiverGroupDto?> GetAsync(Guid id)
    {
        return await CallerProvider.GetAsync<ReceiverGroupDto>($"{_prefix}/{id}");
    }

    public async Task CreateAsync(ReceiverGroupCreateUpdateDto input)
    {
        await CallerProvider.PostAsync(_prefix, input);
    }

    public async Task UpdateAsync(Guid id, ReceiverGroupCreateUpdateDto input)
    {
        await CallerProvider.PutAsync($"{_prefix}/{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await CallerProvider.DeleteAsync($"{_prefix}/{id}", null);
    }
}
