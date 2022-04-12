namespace Masa.Mc.Service.Admin.Application.MessageTemplates;

public class MessageTemplateChannelTypeChangedEventHandler
{
    private readonly IMessageTemplateRepository _repository;

    public MessageTemplateChannelTypeChangedEventHandler(IMessageTemplateRepository repository)
    {
        _repository = repository;
    }

    [EventHandler]
    public async Task MessageTemplateChannelTypeChanged(ChannelTypeChangedDomainEvent eto)
    {
        var list = await _repository.GetListAsync(x => x.ChannelId == eto.ChannelId);
        foreach (var item in list)
        {
            item.SetChannelType(eto.ChannelType);
        }
        await _repository.UpdateRangeAsync(list);
    }

}
