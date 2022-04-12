namespace Masa.Mc.Service.Admin.Application.MessageTemplates.Queries;

public record GetListMessageTemplateQuery(GetMessageTemplateInput Input) : Query<PaginatedListDto<MessageTemplateDto>>
{
    public override PaginatedListDto<MessageTemplateDto> Result { get; set; } = default!;

}
