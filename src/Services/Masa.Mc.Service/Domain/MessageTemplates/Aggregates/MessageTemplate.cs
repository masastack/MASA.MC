namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>
{
    public virtual Guid ChannelId { get; protected set; }
    public virtual string DisplayName { get; protected set; } = string.Empty;
    public virtual string Title { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual string Example { get; protected set; } = string.Empty;
    public virtual string TemplateId { get; protected set; } = string.Empty;
    public virtual bool IsJump { get; protected set; }
    public virtual string JumpUrl { get; protected set; } = string.Empty;
    public virtual string Sign { get; protected set; } = string.Empty;
    public virtual MessageTemplateStatus Status { get; protected set; }
    public virtual MessageTemplateAuditStatus AuditStatus { get; protected set; }
    public virtual DateTime? AuditTime { get; protected set; }
    public virtual DateTime? InvalidTime { get; protected set; }
    public virtual string AuditReason { get; protected set; } = string.Empty;
    public virtual int TemplateType { get; protected set; }
    public virtual long DayLimit { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public virtual ICollection<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

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
