namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>
{
    public virtual Guid ChannelId { get; protected set; }
    public virtual string DisplayName { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual string Example { get; protected set; } = string.Empty;
    public virtual string TemplateId { get; protected set; } = string.Empty;
    public virtual bool IsJump { get; protected set; }
    public virtual string JumpUrl { get; protected set; } = string.Empty;
    public virtual string Sign { get; protected set; } = string.Empty;
    public virtual MessageTemplateStatus Status { get; protected set; }
    public virtual MessageTemplateAuditStatus AuditStatus { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public virtual List<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

    public MessageTemplate(Guid channelId, string displayName, string content, string example, string templateId, bool isJump, string jumpUrl, string sign) : this(channelId, displayName, content, example, templateId, isJump, jumpUrl, sign, new List<MessageTemplateItem>())
    {
    }

    public MessageTemplate(
        Guid channelId,
        string displayName,
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
        ChannelId = channelId;
        TemplateId = templateId;
        IsJump = isJump;
        JumpUrl = jumpUrl;
        Sign = sign;
        Status = status;
        AuditStatus = auditStatus;
        IsStatic = isStatic;

        SetContent(displayName, content, example);

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
            existingItem.SetContent(displayText, description);
        }
    }

    public void SetContent(string displayName, string content, string example)
    {
        DisplayName = displayName;
        Content = content;
        Example = example;
    }
}
