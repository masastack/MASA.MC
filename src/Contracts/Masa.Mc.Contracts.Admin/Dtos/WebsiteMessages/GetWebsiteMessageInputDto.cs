// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Mc.Contracts.Admin.Enums.WebsiteMessages;

namespace Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;

public class GetWebsiteMessageInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public WebsiteMessageFilterType? FilterType { get; set; }
    public Guid? ChannelId { get; set; }

    public GetWebsiteMessageInputDto()
    {
    }

    public GetWebsiteMessageInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetWebsiteMessageInputDto(string filter, WebsiteMessageFilterType? filterType, Guid? channelId, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        FilterType = filterType;
        ChannelId = channelId;
    }
}
