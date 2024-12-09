// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.ValueConverters;

public class ReceiversValueConverter : ValueConverter<List<MessageTaskReceiver>, string>
{
    public ReceiversValueConverter()
        : base(
            d => SerializeObject(d),
            s => DeserializeObject(s))
    {

    }

    private static string SerializeObject(List<MessageTaskReceiver> extraProperties)
    {
        return JsonSerializer.Serialize(extraProperties);
    }

    private static List<MessageTaskReceiver> DeserializeObject(string extraPropertiesAsJson)
    {
        if (string.IsNullOrEmpty(extraPropertiesAsJson) || extraPropertiesAsJson == "[]")
        {
            return new List<MessageTaskReceiver>();
        }
        var result = JsonSerializer.Deserialize<List<MessageTaskReceiver>>(extraPropertiesAsJson) ?? new List<MessageTaskReceiver>();
        return result;
    }
}