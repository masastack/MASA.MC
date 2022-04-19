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
    public int TemplateType { get; protected set; }
    public long DayLimit { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public ICollection<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

    public MessageTemplate(Guid channelId, string displayName, string title, string content, string example, string templateId, bool isJump, string jumpUrl, string sign, int templateType,
        long dayLimit) : this(channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign, templateType, dayLimit, new List<MessageTemplateItem>())
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
        int templateType,
        long dayLimit,
        List<MessageTemplateItem> items,
        MessageTemplateStatus status = MessageTemplateStatus.Normal,
        MessageTemplateAuditStatus auditStatus = MessageTemplateAuditStatus.WaitAudit,
        bool isStatic = false)
    {
        Status = status;
        AuditStatus = auditStatus;
        IsStatic = isStatic;

        SetContent(channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign, templateType, dayLimit);

        Items = items ?? new List<MessageTemplateItem>();
    }

    public virtual void AddOrUpdateItem(string code, string mappingCode, string displayText, string description, bool isStatic = false)
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

    public virtual void SetContent(
        Guid channelId,
        string displayName,
        string title,
        string content,
        string example,
        string templateId,
        bool isJump,
        string jumpUrl,
        string sign,
        int templateType,
        long dayLimit)
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
        TemplateType = templateType;
        DayLimit = dayLimit;
    }

    public virtual void SetAuditStatus(MessageTemplateAuditStatus auditStatus, string auditReason = "")
    {
        AuditStatus = auditStatus;
        AuditTime = DateTime.UtcNow;
        AuditReason = auditReason;
    }

    public virtual void SetInvalid()
    {
        InvalidTime = DateTime.UtcNow;
        Status = MessageTemplateStatus.Invalid;
    }
}
