namespace Masa.Mc.Caller.Services.MessageTemplates;

public class MessageTemplateService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal MessageTemplateService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/message-template";
    }

    public async Task<PaginatedListDto<MessageTemplateDto>> GetListAsync(GetMessageTemplateInput input)
    {
        return await GetAsync<GetMessageTemplateInput,PaginatedListDto<MessageTemplateDto>>(string.Empty, input) ?? new();
    }

    public async Task<MessageTemplateDto?> GetAsync(Guid id)
    {
        return await GetAsync<MessageTemplateDto>($"{id}");
    }

    public async Task CreateAsync(MessageTemplateCreateUpdateDto input)
    {
        await PostAsync(string.Empty, input);
    }

    public async Task UpdateAsync(Guid id, MessageTemplateCreateUpdateDto input)
    {
        await PutAsync($"{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await DeleteAsync($"{id}");
    }

    public async Task<MessageTemplateDto?> FindByCodeAsync(string code)
    {
        return await GetAsync<MessageTemplateDto>($"FindByCode?code={code}");
    }
}
