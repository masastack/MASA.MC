// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class GetMessageRecordInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public bool? Success { get; set; }
    public MessageRecordTimeTypes? TimeType { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public GetMessageRecordInputDto()
    {
    }

    public GetMessageRecordInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageRecordInputDto(string filter, bool? success, MessageRecordTimeTypes? timeType,
       DateTime? startTime, DateTime? endTime, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        Success = success;
        TimeType = timeType;
        StartTime = startTime;
        EndTime = endTime;
    }
}
