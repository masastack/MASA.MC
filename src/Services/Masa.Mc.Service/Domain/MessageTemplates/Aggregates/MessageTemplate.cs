namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>
{
    public Guid ChannelId { get; protected set; }
    public string DisplayName { get; protected set; } = string.Empty;
    public string Title { get; protected set; } = string.Empty;
    public string Content { get; protected set; } = string.Empty;
    public string Example { get; protected set; } = string.Empty;
    public string TemplateId { get; protected set; } = string.Empty;
    public bool IsJump { get; protected set; }
    public string JumpUrl { get; protected set; } = string.Empty;
    public string Sign { get; protected set; } = string.Empty;
    public MessageTemplateStatus Status { get; protected set; }
    public MessageTemplateAuditStatus AuditStatus { get; protected set; }
    public DateTime? AuditTime { get; protected set; }
    public DateTime? InvalidTime { get; protected set; }
    public string AuditReason { get; protected set; } = string.Empty;
    public bool IsStatic { get; protected set; }
    public List<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

    public MessageTemplate(Guid channelId, string displayName, string title, string content, string example, string templateId, bool isJump, string jumpUrl, string sign) : this(channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign, new List<MessageTemplateItem>())
    {
    }

    public MessageTemplate(
        Guid channelId,
        string displayName,
        string title,
        string content,
        string example,
        string templateId,
        bool isJump,
        string jumpUrl,
        string sign,
        List<MessageTemplateItem> items,
        MessageTemplateStatus status = MessageTemplateStatus.Normal,
        MessageTemplateAuditStatus auditStatus = MessageTemplateAuditStatus.WaitAudit,
        bool isStatic = false)
    {
        Status = status;
        AuditStatus = auditStatus;
        IsStatic = isStatic;

        SetContent(channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign);

        Items = items ?? new List<MessageTemplateItem>();
    }

    public void AddOrUpdateItem(string code, string mappingCode, string displayText, string description, bool isStatic = false)
    {
        var existingItem = Items.SingleOrDefault(item => item.Code == code);

        if (existingItem == null)
        {
            Items.Add(new MessageTemplateItem(Id, code, mappingCode, displayText, description, isStatic));
        }
        else
        {
            existingItem.SetContent(mappingCode, displayText, description);
        }
    }

    public void SetContent(
        Guid channelId,
        string displayName,
        string title,
        string content,
        string example,
        string templateId,
        bool isJump,
        string jumpUrl,
        string sign)
    {
        ChannelId = channelId;
        DisplayName = displayName;
        Title = title;
        Content = content;
        Example = example;
        TemplateId = templateId;
        IsJump = isJump;
        JumpUrl = jumpUrl;
        Sign = sign;
    }

    public void SetAuditStatus(MessageTemplateAuditStatus auditStatus, string auditReason = "")
    {
        AuditStatus = auditStatus;
        AuditTime = DateTime.UtcNow;
        AuditReason = auditReason;
    }

    public void SetInvalid()
    {
        InvalidTime = DateTime.UtcNow;
        Status = MessageTemplateStatus.Invalid;
    }
}
