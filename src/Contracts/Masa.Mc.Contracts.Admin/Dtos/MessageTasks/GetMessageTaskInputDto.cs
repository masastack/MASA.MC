// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; }
    public MessageEntityTypes? EntityType { get; set; }
    public bool? IsEnabled { get; set; }
    public MessageTaskTimeTypes? TimeType { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }

    public GetMessageTaskInputDto()
    {

    }

    public GetMessageTaskInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTaskInputDto(string filter, Guid? channelId, MessageEntityTypes? entityType, bool? isEnabled, MessageTaskTimeTypes? timeType, DateTimeOffset? startTime, DateTimeOffset? endTime, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        ChannelId = channelId;
        EntityType = entityType;
        IsEnabled = isEnabled;
        TimeType = timeType;
        StartTime = startTime;
        EndTime = endTime;
    }
}
