﻿namespace Masa.Mc.Infrastructure.EntityFrameworkCore.EntityFrameworkCore.ValueConverters;

public class ExtraPropertiesValueConverter : ValueConverter<ExtraPropertyDictionary, string>
{
    public ExtraPropertiesValueConverter()
        : base(
            d => SerializeObject(d),
            s => DeserializeObject(s))
    {

    }

    private static string SerializeObject(ExtraPropertyDictionary extraProperties)
    {
        return JsonSerializer.Serialize(extraProperties);
    }

    private static ExtraPropertyDictionary DeserializeObject(string extraPropertiesAsJson)
    {
        if (string.IsNullOrEmpty(extraPropertiesAsJson) || extraPropertiesAsJson == "{}")
        {
            return new ExtraPropertyDictionary();
        }
        return JsonSerializer.Deserialize<ExtraPropertyDictionary>(extraPropertiesAsJson) ?? new ExtraPropertyDictionary();
    }
}