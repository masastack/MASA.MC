using MASA.MC.Contracts.Admin.Enums.Channels;

namespace MASA.MC.Contracts.Admin.Dtos.Channels;

public class GetChannelInput : PaginatedOptionsDto
{
    public ChannelType Type { get; set; }
    public GetChannelInput(int page, int pageSize, ChannelType type)
    {
        Page = page;
        PageSize = pageSize;
        Type = type;
    }
}
