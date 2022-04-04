using MASA.MC.Contracts.Admin.Enums.NotificationTemplates;

namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;
public class NotificationTemplate : AuditAggregateRoot<Guid, Guid?>
{
    public virtual Guid ChannelId { get; protected set; }
    public virtual string DisplayName { get; protected set; } = string.Empty;
    public virtual string Content { get; protected set; } = string.Empty;
    public virtual string Example { get; protected set; } = string.Empty;
    public virtual string TemplateId { get; protected set; } = string.Empty;
    public virtual NotificationTemplateStatus Status { get; protected set; }
    public virtual bool IsStatic { get; protected set; }
    public virtual ICollection<NotificationTemplateItem> Items { get; protected set; } = new List<NotificationTemplateItem>();
    private NotificationTemplate()
    {
    }
    public NotificationTemplate(
        string displayName,
        string content,
        string example,
        List<NotificationTemplateItem> items,
        NotificationTemplateStatus status = NotificationTemplateStatus.Normal,
        bool isStatic = false)
    {
        Status = status;
        IsStatic = isStatic;

        SetContent(displayName, content, example);

        Items = items ?? new List<NotificationTemplateItem>();
    }
    public void AddOrUpdateItem(string code, string mappingCode, string displayText, string description, bool isStatic = false)
    {
        var existingItem = Items.SingleOrDefault(item => item.Code == code);

        if (existingItem == null)
        {
            Items.Add(new NotificationTemplateItem(Id, code, mappingCode,displayText, description, isStatic));
        }
        else
        {
            existingItem.SetContent(displayText, description);
        }
    }

    public void SetContent(string displayName, string content,string example)
    {
        DisplayName = displayName;
        Content = content;
        Example = example;
    }
}
