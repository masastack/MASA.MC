// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class GetMessageTemplateInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public MessageTemplateStatuses? Status { get; set; }
    public MessageTemplateAuditStatuses? AuditStatus { get; set; }
    public ChannelTypes? ChannelType { get; set; }
    public Guid? ChannelId { get; set; }
    public DateTimeOffset? StartTime { get; set; }
    public DateTimeOffset? EndTime { get; set; }
    public int TemplateType { get; set; }

    public GetMessageTemplateInputDto()
    {

    }

    public GetMessageTemplateInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTemplateInputDto(string filter, ChannelTypes? channelType, Guid? channelId, MessageTemplateStatuses? status, MessageTemplateAuditStatuses? auditStatus, DateTimeOffset? startTime, DateTimeOffset? endTime, int templateType, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
    {
        Filter = filter;
        ChannelType = channelType;
        ChannelId = channelId;
        Status = status;
        AuditStatus = auditStatus;
        StartTime = startTime;
        EndTime = endTime;
        TemplateType = templateType;
    }
}
