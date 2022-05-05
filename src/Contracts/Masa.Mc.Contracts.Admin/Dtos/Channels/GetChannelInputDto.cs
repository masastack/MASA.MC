// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Channels;

public class GetChannelInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public ChannelTypes? Type { get; set; }
    public string DisplayName { get; set; } = string.Empty;

    public GetChannelInputDto()
    {
    }

    public GetChannelInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetChannelInputDto(string filter, ChannelTypes? type, string displayName,
       string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        Type = type;
        DisplayName = displayName;
    }

    public static ValueTask<GetChannelInputDto?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        var filter = httpContext.Request.Query["filter"];
        Enum.TryParse<ChannelTypes>(httpContext.Request.Query["type"], out var type);
        var displayName = httpContext.Request.Query["displayName"];
        var sorting = httpContext.Request.Query["sorting"];
        int.TryParse(httpContext.Request.Query["page"], out var page);
        int.TryParse(httpContext.Request.Query["pageSize"], out var pageSize);

        return ValueTask.FromResult<GetChannelInputDto?>(
            new GetChannelInputDto(
                filter,
                type,
                displayName,
                sorting,
                page == 0 ? 1 : page,
                pageSize == 0 ? 20 : pageSize
            )
        );
    }
}
