namespace MASA.MC.Service.Admin.Domain.MessageTemplates.Aggregates;

public class MessageTemplateItem : Entity<Guid>
{
    public virtual Guid MessageTemplateId { get; protected set; }

    public virtual string Code { get; protected set; } = string.Empty;

    public virtual string MappingCode { get; protected set; } = string.Empty;

    public virtual string DisplayText { get; protected set; } = string.Empty;

    public virtual string Description { get; protected set; } = string.Empty;

    public virtual bool IsStatic { get; protected set; }

    //public override IEnumerable<(string Name, object Value)> GetKeys()
    //{
    //    yield return ("MessageTemplateId", MessageTemplateId!);
    //    yield return ("Code", Code!);
    //}

    public MessageTemplateItem(Guid messageTemplateId, string code): this(messageTemplateId, code,"","","",false)
    {

    }

    public MessageTemplateItem(
        Guid messageTemplateId,
        string code,
        string mappingCode,
        string displayText,
        string description,
        bool isStatic)
    {
        MessageTemplateId = messageTemplateId;
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
