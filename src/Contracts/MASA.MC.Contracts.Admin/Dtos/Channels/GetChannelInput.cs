using MASA.MC.Contracts.Admin.Enums.Channels;
using Microsoft.AspNetCore.Http;
using System.Reflection;

namespace MASA.MC.Contracts.Admin.Dtos.Channels;

public class GetChannelInput : PaginatedOptionsDto
{
    public ChannelType? Type { get; set; }
    public string DisplayName { get; set; }
    public GetChannelInput(ChannelType? type, string displayName, 
       string sorting, int page, int pageSize):base(sorting, page, pageSize)
    {
        Type = type;
        DisplayName = displayName;
    }
    public static ValueTask<GetChannelInput?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        Enum.TryParse<ChannelType>(httpContext.Request.Query["type"], out var type);
        var displayName = httpContext.Request.Query["displayName"];
        var sorting = httpContext.Request.Query["sorting"];
        int.TryParse(httpContext.Request.Query["page"], out var page);
        int.TryParse(httpContext.Request.Query["pageSize"], out var pageSize);

        return ValueTask.FromResult<GetChannelInput?>(
            new GetChannelInput(
                type,
                displayName,
                sorting,
                page == 0 ? 1 : page,
                pageSize == 0 ? 20 : pageSize
            )
        );
    }
}
