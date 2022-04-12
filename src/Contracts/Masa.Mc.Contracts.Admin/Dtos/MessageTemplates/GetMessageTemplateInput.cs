namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class GetMessageTemplateInput : PaginatedOptionsDto
{
    public GetMessageTemplateInput(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }
}
