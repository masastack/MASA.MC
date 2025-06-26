// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Channels.Aggregates;

public class Channel : FullAggregateRoot<Guid, Guid>
{
    public string DisplayName { get; protected set; } = string.Empty;

    public string Color { get; protected set; } = string.Empty;

    public string Code { get; protected set; } = string.Empty;

    public ChannelType Type { get; } = default!;

    public string Description { get; protected set; } = string.Empty;

    public bool IsStatic { get; protected set; }

    public string Scheme { get; protected set; }

    public string SchemeField { get; protected set; }

    public int Provider { get; protected set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; } = new();

    public Channel(string displayName, string color, string code, ChannelType type, string description) : this(displayName, color, code, type, description, new Dictionary<string, string>())
    {
    }

    public Channel(
        string displayName,
        string color,
        string code,
        ChannelType type,
        string description,
        Dictionary<string, string> extraProperties,
        bool isStatic = false
        )
    {
        DisplayName = displayName;
        Color = color;
        Code = code;
        Type = type;
        Description = description;
        IsStatic = isStatic;
        foreach (var p in extraProperties)
        {
            SetDataValue(p.Key, p.Value);
        }
    }

    private Channel() { }

    public virtual object GetDataValue(string name)
    {
        return ExtraProperties.GetProperty(name);
    }

    public virtual T GetDataValue<T>(string name)
    {
        return ExtraProperties.GetProperty<T>(name);
    }

    public virtual void SetDataValue(string name, string value)
    {
        ExtraProperties.SetProperty(name, value);
    }

    public void Remove()
    {
        AddDomainEvent(new RemoveChannelTemplatesDomainEvent(Id));
        AddDomainEvent(new RemoveChannelMessageTasksDomainEvent(Id));
    }
}