namespace Masa.Mc.Service.Admin.Services;

public class SmsTemplateService : ServiceBase
{
    public SmsTemplateService(IServiceCollection services) : base(services, "api/sms-template")
    {
        MapGet(GetListByChannelIdAsync);
        MapPost(SynchroAsync);
    }

    public async Task<List<SmsTemplateDto>> GetListByChannelIdAsync(IEventBus eventbus, Guid channelId)
    {
        var query = new GetSmsTemplateListByChannelIdQuery(channelId);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task SynchroAsync(IEventBus eventbus, SmsTemplateSynchroInput input)
    {
        var command = new SynchroSmsTemplateCommand(input.ChannelId);
        await eventbus.PublishAsync(command); 
    }
}