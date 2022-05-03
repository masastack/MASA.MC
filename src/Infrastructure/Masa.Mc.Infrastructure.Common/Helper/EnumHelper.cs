// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Helper;

public class EnumHelper
{
    public static List<TEnum> GetEnumList<TEnum>() where TEnum : struct, Enum
    {
        return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
    }
}
