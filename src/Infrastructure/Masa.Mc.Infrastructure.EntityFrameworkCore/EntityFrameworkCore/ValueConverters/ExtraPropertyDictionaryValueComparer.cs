// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.EntityFrameworkCore.ValueConverters;

public class ExtraPropertyDictionaryValueComparer : ValueComparer<ExtraPropertyDictionary>
{
    public ExtraPropertyDictionaryValueComparer()
        : base(
              (d1, d2) => d1.SequenceEqual(d2),
              d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())),
              d => new ExtraPropertyDictionary(d))
    {
    }
}
