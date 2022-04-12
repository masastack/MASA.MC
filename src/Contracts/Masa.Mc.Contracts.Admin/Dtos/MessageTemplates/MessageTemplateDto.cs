namespace Masa.Mc.Contracts.Admin.Dtos.MessageTemplates;

public class MessageTemplateDto : AuditEntityDto<Guid, Guid>
{
    public MessageTemplateDto()
    {
        this.Items = new List<MessageTemplateItemDto>();
    }
    public ChannelType ChannelType { get; set; }
    public Guid ChannelId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string TemplateId { get; set; } = string.Empty;
    public bool IsJump { get; set; }
    public string JumpUrl { get; set; } = string.Empty;
    public string Sign { get; set; } = string.Empty;
    public MessageTemplateStatus Status { get; set; }
    public MessageTemplateAuditStatus AuditStatus { get; set; }
    public bool IsStatic { get; set; }
    public ICollection<MessageTemplateItemDto> Items { get; set; }
}