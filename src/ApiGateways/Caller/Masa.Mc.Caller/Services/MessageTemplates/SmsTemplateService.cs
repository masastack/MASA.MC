namespace Masa.Mc.Caller.Services.MessageTemplates;

public class SmsTemplateService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal SmsTemplateService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sms-template";
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(Guid channelId)
    {
        return await GetAsync<List<SmsTemplateDto>>($"GetListByChannelId?channelId={channelId}") ?? new();
    }

    public async Task SynchroAsync(SmsTemplateSynchroInput input)
    {
        await PostAsync("Synchro", input);
    }
}
