namespace MASA.MC.Contracts.Admin.Dtos.MessageTemplates;

public class GetMessageTemplateInput : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;

    public GetMessageTemplateInput()
    {

    }

    public GetMessageTemplateInput(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTemplateInput(string filter, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
    }
}
