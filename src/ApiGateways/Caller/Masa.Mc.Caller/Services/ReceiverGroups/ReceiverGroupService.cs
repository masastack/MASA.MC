namespace Masa.Mc.Caller.Services.ReceiverGroups;

public class ReceiverGroupService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    public ReceiverGroupService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/receiver-group";
    }

    public async Task<PaginatedListDto<ReceiverGroupDto>> GetListAsync(GetReceiverGroupInput input)
    {
        return await GetAsync<GetReceiverGroupInput, PaginatedListDto<ReceiverGroupDto>>(string.Empty, input) ?? new();
    }

    public async Task<ReceiverGroupDto?> GetAsync(Guid id)
    {
        return await GetAsync<ReceiverGroupDto>($"{id}");
    }

    public async Task CreateAsync(ReceiverGroupCreateUpdateDto input)
    {
        await PostAsync(string.Empty, input);
    }

    public async Task UpdateAsync(Guid id, ReceiverGroupCreateUpdateDto input)
    {
        await PutAsync($"{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }
}
