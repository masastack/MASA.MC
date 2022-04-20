namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetMessageTemplateQuery(Guid MessageTemplateId) : Query<MessageTemplateDto>
{
    public override MessageTemplateDto Result { get; set; } = new();
}
