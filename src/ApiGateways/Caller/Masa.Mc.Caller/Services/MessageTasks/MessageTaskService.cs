namespace Masa.Mc.Caller.Services.MessageTasks;

public class MessageTaskService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageTaskService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-task";
    }

    public async Task<PaginatedListDto<MessageTaskDto>> GetListAsync(GetMessageTaskInput input)
    {
        return await GetAsync<GetMessageTaskInput, PaginatedListDto<MessageTaskDto>>(string.Empty, input) ?? new();
    }

    public async Task<MessageTaskDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageTaskDto>($"{id}");
    }

    public async Task CreateAsync(MessageTaskCreateUpdateDto input)
    {
        await PostAsync(string.Empty, input);
    }

    public async Task UpdateAsync(Guid id, MessageTaskCreateUpdateDto input)
    {
        await PutAsync($"{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task ExecuteAsync(ExecuteMessageTaskInput input)
    {
        await PostAsync("Execute", input);
    }
}
