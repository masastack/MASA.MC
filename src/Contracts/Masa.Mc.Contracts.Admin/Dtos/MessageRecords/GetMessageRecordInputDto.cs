// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageRecords;

public class GetMessageRecordInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public Guid? ChannelId { get; set; }
    public bool? Success { get; set; }
    public MessageRecordTimeTypes? TimeType { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public Guid? UserId { get; set; }
    public Guid? MessageTemplateId { get; set; }
    public Guid? MessageTaskHistoryId { get; set; }

    public GetMessageRecordInputDto()
    {
    }

    public GetMessageRecordInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageRecordInputDto(string filter, Guid? channelId, bool? success, MessageRecordTimeTypes? timeType,
       DateTime? startTime, DateTime? endTime, Guid? userId, Guid? messageTemplateId, Guid? messageTaskHistoryId, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        ChannelId = channelId;
        Success = success;
        TimeType = timeType;
        StartTime = startTime;
        EndTime = endTime;
        UserId = userId;
        MessageTemplateId = messageTemplateId;
        MessageTaskHistoryId = messageTaskHistoryId;
    }
}
