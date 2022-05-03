// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTasks;

public class GetMessageTaskHistoryInput : PaginatedOptionsDto
{
    public MessageTaskHistoryStatus? Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public GetMessageTaskHistoryInput()
    {

    }

    public GetMessageTaskHistoryInput(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTaskHistoryInput(MessageTaskHistoryStatus? status, DateTime? startTime, DateTime? endTime, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Status = status;
        StartTime = startTime;
        EndTime = endTime;
    }
}
