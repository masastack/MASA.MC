namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    public SmsTemplateService(IServiceCollection services) : base(services, "api/sms-template")
    {
        MapGet(GetListByChannelIdAsync);
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }
}
