namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetSmsTemplateQuery(Guid ChannelId,string TemplateCode) : Query<SmsTemplateDto>
{
    public override SmsTemplateDto Result { get; set; } = new();
}
