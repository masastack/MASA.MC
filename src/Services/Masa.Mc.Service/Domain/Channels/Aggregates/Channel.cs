namespace Masa.Mc.Service.Admin.Domain.Channels.Aggregates;

public class Channel : AuditAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;
    public string Code { get; protected set; } = string.Empty;
    public ChannelType Type { get; protected set; }
    public string Description { get; protected set; } = string.Empty;
    public bool IsStatic { get; protected set; }
    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    public Channel(string displayName,string code,ChannelType type,string description) : this(displayName, code, type, description, new Dictionary<string, string>())
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
        Code = code;
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

