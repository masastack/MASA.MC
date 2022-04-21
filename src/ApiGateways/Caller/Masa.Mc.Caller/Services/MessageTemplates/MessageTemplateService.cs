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
        var queryArguments = new Dictionary<string, string?>()
        {
            { "channelType", input.ChannelType?.ToString() },
            { "channelId", input.ChannelId?.ToString() },
            { "auditStatus", input.AuditStatus?.ToString() },
            { "status", input.Status?.ToString() },
            { "startTime", input.StartTime?.ToString() },
            { "endTime", input.EndTime?.ToString() },
            { "templateType", input.TemplateType.ToString() },
            { "filter", input.Filter.ToString() },
            { "sorting", input.Sorting.ToString() },
            { "page", input.Page.ToString() },
            { "pageSize", input.PageSize.ToString() }
        };
        var url = QueryHelpers.AddQueryString(string.Empty, queryArguments);
        return await GetAsync<PaginatedListDto<MessageTemplateDto>>(url) ?? new();
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
