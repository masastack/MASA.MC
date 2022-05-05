// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class GetMessageTemplateInputDto : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public MessageTemplateStatues? Status { get; set; }
    public MessageTemplateAuditStatues? AuditStatus { get; set; }
    public ChannelTypes? ChannelType { get; set; }
    public Guid? ChannelId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int TemplateType { get; set; }

    public GetMessageTemplateInputDto()
    {

    }

    public GetMessageTemplateInputDto(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTemplateInputDto(string filter, ChannelTypes? channelType, Guid? channelId, MessageTemplateStatues? status, MessageTemplateAuditStatues? auditStatus, DateTime? startTime, DateTime? endTime, int templateType, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
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
