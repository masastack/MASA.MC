// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.Collections.Concurrent;

public static class ExtensionPropertyHelper
{
    public static ExtraPropertyDictionary ObjMapToExtraProperty(object obj)
    {
        var source = new ExtraPropertyDictionary();
        var properties = obj.GetType().GetProperties();
        foreach (var property in properties)
        {
            if (source.ContainsKey(property.Name))
            {
                source[property.Name] = property.GetValue(obj);
            }
            else
            {
                source.TryAdd(property.Name, property.GetValue(obj));
            }
        }
        return source;
    }

    public static T ExtraPropertyMapToObj<T>(ExtraPropertyDictionary dic) where T : new()
    {
        Type myType = typeof(T);
        T entity = new T();
        var fields = myType.GetProperties();
        string val = string.Empty;
        object obj = null;

        foreach (var field in fields)
        {
            if (!dic.ContainsKey(field.Name))
                continue;
            val = dic[field.Name]?.ToString();
            if (val == null) continue;
            object defaultVal;
            if (field.PropertyType.Name.Equals("String"))
                defaultVal = "";
            else if (field.PropertyType.Name.Equals("Boolean"))
            {
                defaultVal = false;
                //val = (val.Equals("1") || val.Equals("on")).ToString();
            }
            else if (field.PropertyType.Name.Equals("Decimal"))
                defaultVal = 0M;
            else
                defaultVal = 0;

            if (!field.PropertyType.IsGenericType)
                obj = string.IsNullOrEmpty(val) ? defaultVal : Convert.ChangeType(val, field.PropertyType);
            else
            {
                Type genericTypeDefinition = field.PropertyType.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof(Nullable<>))
                    obj = string.IsNullOrEmpty(val) ? defaultVal : Convert.ChangeType(val, Nullable.GetUnderlyingType(field.PropertyType));
            }
            field.SetValue(entity, obj, null);
        }
        return entity;
    }

    public static T ConvertToType<T>(ExtraPropertyDictionary dic) where T : class
    {
        var extraPropertiesAsJson = JsonSerializer.Serialize(dic);
        if (string.IsNullOrEmpty(extraPropertiesAsJson) || extraPropertiesAsJson == "{}")
        {
            return Activator.CreateInstance<T>();
        }
        return JsonSerializer.Deserialize<T>(extraPropertiesAsJson) ?? Activator.CreateInstance<T>();
    }
}
