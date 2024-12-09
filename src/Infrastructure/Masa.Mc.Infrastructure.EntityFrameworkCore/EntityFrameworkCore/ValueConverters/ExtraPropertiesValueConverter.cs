// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.EntityFrameworkCore.ValueConverters;

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