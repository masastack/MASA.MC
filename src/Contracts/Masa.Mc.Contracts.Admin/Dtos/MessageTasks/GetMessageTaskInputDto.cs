// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; }
    public MessageEntityTypes? EntityType { get; set; }
    public bool? IsDraft { get; set; }
    public bool? IsEnabled { get; set; }
    public MessageTaskTimeTypes? TimeType { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public MessageTaskStatuses? Status { get; set; }

    public GetMessageTaskInputDto()
    {

    }

    public GetMessageTaskInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTaskInputDto(string filter, Guid? channelId, MessageEntityTypes? entityType, bool? isDraft, bool? isEnabled, MessageTaskTimeTypes? timeType, DateTime? startTime, DateTime? endTime, MessageTaskStatuses? status, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        ChannelId = channelId;
        EntityType = entityType;
        IsDraft = isDraft;
        IsEnabled = isEnabled;
        TimeType = timeType;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }
}
