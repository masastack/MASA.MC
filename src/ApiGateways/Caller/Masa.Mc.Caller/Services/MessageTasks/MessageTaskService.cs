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

    public async Task SendAsync(SendMessageTaskInput input)
    {
        await PostAsync("Send", input);
    }

    public async Task SendTestAsync(SendTestMessageTaskInput input)
    {
        await PostAsync("SendTest", input);
    }

    public async Task WithdrawnHistoryAsync(WithdrawnMessageTaskHistoryInput input)
    {
        await PostAsync("WithdrawnHistory", input);
    }

    public async Task EnabledAsync(EnabledMessageTaskInput input)
    {
        await PostAsync("Enabled", input);
    }

    public async Task DisableAsync(DisableMessageTaskInput input)
    {
        await PostAsync("Disable", input);
    }
}
