namespace MASA.MC.Service.Admin.Domain.Channels.Aggregates;

public class Channel : AuditAggregateRoot<Guid, Guid?>
{
    public virtual string DisplayName { get; protected set; } = string.Empty;
    public virtual string Code { get; protected set; } = string.Empty;
    public virtual ChannelType Type { get; protected set; }
    public virtual string Description { get; protected set; } = string.Empty;
    public virtual bool IsStatic { get; protected set; }
    public virtual ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    private Channel()
    {
    }

    public Channel(
        string displayName,
        string code,
        ChannelType type,
        string description,
        Dictionary<string, string> extraProperties,
        bool isStatic = false)
    {
        DisplayName = displayName;
        Code=code;
        Type = type;
        Description = description;
        IsStatic = isStatic;
        foreach (var p in extraProperties)
        {
            SetDataValue(p.Key, p.Value);
        }
    }

    public object? GetDataValue(string name)
    {
        return ExtraProperties?.GetOrDefault(name);
    }

    public void SetDataValue(string name, string value)
    {
        ExtraProperties[name] = value;
    }
}

