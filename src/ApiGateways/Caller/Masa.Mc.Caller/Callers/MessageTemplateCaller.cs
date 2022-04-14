namespace Masa.Mc.Caller.Callers;

public class MessageTemplateCaller : HttpClientCallerBase
{
    protected override string BaseAddress { get; set; }
    private readonly string _prefix = "/api/message-template";

    public MessageTemplateCaller(IServiceProvider serviceProvider, IOptions<Settings> settings) : base(serviceProvider)
    {
        BaseAddress = settings.Value.McServiceBaseUrl;
        Name = nameof(MessageTemplateCaller);
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
            { "filter", input.Filter.ToString() },
            { "page", input.Page.ToString() },
            { "pageSize", input.PageSize.ToString() }
        };
        var url = QueryHelpers.AddQueryString(_prefix, queryArguments);
        return await CallerProvider.GetAsync<PaginatedListDto<MessageTemplateDto>>(url) ?? new();
    }

    public async Task<MessageTemplateDto?> GetAsync(Guid id)
    {
        return await CallerProvider.GetAsync<MessageTemplateDto>($"{_prefix}/{id}");
    }

    public async Task CreateAsync(MessageTemplateCreateUpdateDto input)
    {
        await CallerProvider.PostAsync(_prefix, input);
    }

    public async Task UpdateAsync(Guid id, MessageTemplateCreateUpdateDto input)
    {
        await CallerProvider.PutAsync($"{_prefix}/{id}", input);
    }

    public async Task DeleteAsync(Guid id)
    {
        await CallerProvider.DeleteAsync($"{_prefix}/{id}", null);
    }

    public async Task<MessageTemplateDto?> FindByCodeAsync(string code)
    {
        return await CallerProvider.GetAsync<MessageTemplateDto>($"{_prefix}/FindByCode?code={code}");
    }

    public async Task<SmsTemplateDto?> GetSmsTemplateAsync(Guid channelId, string templateCode)
    {
        var queryArguments = new Dictionary<string, string?>()
        {
            { "channelId", channelId.ToString() },
            { "templateCode", templateCode }
        };
        var url = QueryHelpers.AddQueryString($"{_prefix}/GetSmsTemplate", queryArguments);
        return await CallerProvider.GetAsync<SmsTemplateDto>(url);
    }
}
