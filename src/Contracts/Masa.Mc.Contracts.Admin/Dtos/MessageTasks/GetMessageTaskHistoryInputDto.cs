// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskHistoryInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public Guid MessageTaskId { get; set; }
    public MessageTaskHistoryStatuses? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public GetMessageTaskHistoryInputDto()
    {

    }

    public GetMessageTaskHistoryInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTaskHistoryInputDto(string filter, Guid messageTaskId, MessageTaskHistoryStatuses? status, DateTime? startTime, DateTime? endTime, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        MessageTaskId = messageTaskId;
        Status = status;
        StartTime = startTime;
        EndTime = endTime;
    }
}
