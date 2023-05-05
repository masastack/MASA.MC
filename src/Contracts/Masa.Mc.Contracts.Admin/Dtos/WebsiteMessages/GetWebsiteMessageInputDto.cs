// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.WebsiteMessages;

public class GetWebsiteMessageInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;

    public WebsiteMessageFilterType? FilterType { get; set; }

    public Guid? ChannelId { get; set; }

    public string ChannelCode { get; set; } = string.Empty;

    public bool? IsRead { get; set; }

    public string Tag { get; set; } = string.Empty;

    public GetWebsiteMessageInputDto()
    {
    }

    public GetWebsiteMessageInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetWebsiteMessageInputDto(string filter, WebsiteMessageFilterType? filterType, Guid? channelId, string channelCode, bool? isRead, string tag, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        FilterType = filterType;
        ChannelId = channelId;
        ChannelCode = channelCode;
        IsRead = isRead;
        Tag = tag;
    }
}
