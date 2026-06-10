// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.Unsubscriptions;

public class GetUnsubscriptionHistoryInputDto : PaginatedOptionsDto
{
    public Guid? ChannelId { get; set; }

    public Guid? UserId { get; set; }

    public string Filter { get; set; } = string.Empty;

    public string ChannelUserIdentity { get; set; } = string.Empty;

    public string ScopeRefId { get; set; } = string.Empty;

    public string Keyword { get; set; } = string.Empty;

    public UnsubscriptionSource? Source { get; set; }

    public UnsubscriptionTimelineActions? Action { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public DateTimeOffset? EndTime { get; set; }

    public GetUnsubscriptionHistoryInputDto()
    {
    }

    public GetUnsubscriptionHistoryInputDto(
        Guid? channelId,
        Guid? userId,
        string filter,
        string channelUserIdentity,
        string scopeRefId,
        string keyword,
        UnsubscriptionSource? source,
        UnsubscriptionTimelineActions? action,
        DateTimeOffset? startTime,
        DateTimeOffset? endTime,
        string sorting,
        int page,
        int pageSize) : base(sorting, page, pageSize)
    {
        ChannelId = channelId;
        UserId = userId;
        Filter = filter;
        ChannelUserIdentity = channelUserIdentity;
        ScopeRefId = scopeRefId;
        Keyword = keyword;
        Source = source;
        Action = action;
        StartTime = startTime;
        EndTime = endTime;
    }
}
