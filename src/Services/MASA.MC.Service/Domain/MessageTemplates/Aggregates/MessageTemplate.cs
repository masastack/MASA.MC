namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Aggregates;
public class MessageTemplate : AuditAggregateRoot<Guid, Guid>
{
    public virtual Guid ChannelId { get; protected set; }
    public virtual string DisplayName { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual string Example { get; protected set; } = string.Empty;
    public virtual string TemplateId { get; protected set; } = string.Empty;
    public virtual MessageTemplateStatus Status { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public virtual ICollection<MessageTemplateItem> Items { get; protected set; } = new List<MessageTemplateItem>();

    public MessageTemplate(string displayName,string content,string example) : this(displayName, content, example, new List<MessageTemplateItem>())
    {
    }

    public MessageTemplate(
        string displayName,
        string content,
        string example,
        List<MessageTemplateItem> items,
        MessageTemplateStatus status = MessageTemplateStatus.Normal,
        bool isStatic = false)
    {
        Status = status;
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
