namespace Masa.Mc.Contracts.Admin.Dtos.ReceiverGroups;

public class GetReceiverGroupInput : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;

    public GetReceiverGroupInput()
    {

    }

    public GetReceiverGroupInput(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetReceiverGroupInput(string filter, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
    }
}
