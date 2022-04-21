namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetSmsTemplateListByChannelIdQuery(Guid ChannelId) : Query<List<SmsTemplateDto>>
{
    public override List<SmsTemplateDto> Result { get; set; } = new();
}
