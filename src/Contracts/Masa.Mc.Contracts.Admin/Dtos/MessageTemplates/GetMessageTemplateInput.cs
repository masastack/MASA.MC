namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class GetMessageTemplateInput : PaginatedOptionsDto
{
    public string Filter { get; set; } = string.Empty;
    public MessageTemplateStatus? Status { get; set; }
    public MessageTemplateAuditStatus? AuditStatus { get; set; }
    public ChannelType? ChannelType { get; set; }
    public Guid? ChannelId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int TemplateType { get; set; }

    public GetMessageTemplateInput()
    {

    }

    public GetMessageTemplateInput(int pageSize) : base("", 1, pageSize)
    {
    }

    public GetMessageTemplateInput(string filter, ChannelType? channelType, Guid? channelId, MessageTemplateStatus? status, MessageTemplateAuditStatus? auditStatus, DateTime? startTime, DateTime? endTime, int templateType, string sorting, int page, int pageSize) : base(sorting, page, pageSize)
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
