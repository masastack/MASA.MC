namespace MASA.MC.Infrastructure.DDD.Application.Contracts.Dtos;

public class PaginatedOptionsDto
{
    public int Page { get; set; }

    public int PageSize { get; set; }

    public string Sorting { get; set; }
    public PaginatedOptionsDto(string sorting = "", int page=1, int pageSize=20)
    {
        Sorting=sorting;
        Page= page;
        PageSize=pageSize;
    }
}
