using System.ComponentModel.DataAnnotations;

namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateCreateUpdateDto
{
    public MessageTemplateCreateUpdateDto()
    {
        this.Items = new List<MessageTemplateItemDto>();
    }
    public Guid ChannelId { get; set; }
    public ChannelType ChannelType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public MessageTemplateStatus Status { get; set; } = MessageTemplateStatus.Normal;
    public MessageTemplateAuditStatus AuditStatus { get; set; } = MessageTemplateAuditStatus.Adopt;
    public DateTime? AuditTime { get; set; }
    public DateTime? InvalidTime { get; set; }
    public string AuditReason { get; set; } = string.Empty;
    public int TemplateType { get; set; }
    public long DayLimit { get; set; }
    public bool IsStatic { get; set; }
    public List<MessageTemplateItemDto> Items { get; set; }
    public MessageInfoCreateUpdateDto MessageInfo { get; set; }
}