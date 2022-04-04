namespace MASA.MC.Service.Admin.Domain.NotificationTemplates.Aggregates;

public class NotificationTemplateItem : Entity
{
    public virtual Guid NotificationTemplateId { get; protected set; }

    public virtual string Code { get; protected set; } = string.Empty;

    public virtual string MappingCode { get; protected set; } = string.Empty;

    public virtual string DisplayText { get; protected set; } = string.Empty;

    public virtual string Description { get; protected set; } = string.Empty;

    public virtual bool IsStatic { get; protected set; }

    public override IEnumerable<(string Name, object Value)> GetKeys()
    {
        yield return ("NotificationTemplateId", NotificationTemplateId!);
        yield return ("Code", Code!);
    }
    protected NotificationTemplateItem()
    {
    }
    public NotificationTemplateItem(
        Guid notificationTemplateId,
        string code,
        string mappingCode,
        string displayText,
        string description,
        bool isStatic)
    {
        NotificationTemplateId = notificationTemplateId;
        Code = code;
        MappingCode = mappingCode;
        IsStatic = isStatic;

        SetContent(displayText, description);
    }

    public void SetContent(string displayText, string description)
    {
        DisplayText = displayText;
        Description = description; 
    }
}
