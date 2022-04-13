namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetSmsTemplateQuery(Guid ChannelId,string TemplateCode) : Query<GetSmsTemplateDto>
{
    public override GetSmsTemplateDto Result { get; set; } = new();
}
