namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>
{
    public virtual ChannelType ChannelType { get; protected set; }
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
    public virtual bool IsStatic { get; protected set; }
    public virtual List<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

    public MessageTemplate(ChannelType channelType, Guid channelId, string displayName, string title, string content, string example, string templateId, bool isJump, string jumpUrl, string sign) : this(channelType, channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign, new List<MessageTemplateItem>())
    {
    }

    public MessageTemplate(
        ChannelType channelType,
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

        SetContent(channelType, channelId, displayName, title, content, example, templateId, isJump, jumpUrl, sign);

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
        ChannelType channelType,
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
        ChannelType = channelType;
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

    public void SetChannelType(ChannelType channelType)
    {
        ChannelType = channelType;
    }
}
