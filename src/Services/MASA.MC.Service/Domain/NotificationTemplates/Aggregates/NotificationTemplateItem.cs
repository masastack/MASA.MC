namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;

public class NotificationTemplateItem : Entity
{
    public virtual Guid NotificationTemplateId { get; protected set; }
    public virtual string Key { get; protected set; }
    public virtual string MappingKey { get; protected set; }

    public virtual string DisplayText { get; protected set; }

    public virtual string Description { get; protected set; }

    public virtual bool IsStatic { get; protected set; }
    public override IEnumerable<(string Name, object Value)> GetKeys()
    {
        yield return ("NotificationTemplateId", NotificationTemplateId!);
        yield return ("Key", Key!);
    }
    protected NotificationTemplateItem()
    {
    }
    public NotificationTemplateItem(
        Guid notificationTemplateId,
        string key,
        string displayText,
        string description,
        bool isStatic)
    {
        NotificationTemplateId = notificationTemplateId;
        Key = key;
        IsStatic = isStatic;

        SetContent(displayText, description);
    }

    public void SetContent(string displayText, string description)
    {
        DisplayText = displayText;
        Description = description; 
    }
}
