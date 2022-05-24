// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Helper;

public class UtilConvert
{
    public static string GetGuidToNumber()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0).ToString();
    }

    public static dynamic ConvertToDynamic(object obj)
    {
        IDictionary<string, object> result = new ExpandoObject();
        foreach (PropertyDescriptor pro in TypeDescriptor.GetProperties(obj.GetType()))
        {
            result.Add(pro.Name, pro.GetValue(obj));
        }
        return result as ExpandoObject;
    }
}
