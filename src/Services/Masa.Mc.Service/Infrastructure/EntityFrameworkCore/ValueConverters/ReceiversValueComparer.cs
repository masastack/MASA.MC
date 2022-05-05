// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore.ValueConverters;

public class ReceiversValueComparer : ValueComparer<List<MessageTaskReceiver>>
{
    public ReceiversValueComparer()
        : base(
              (d1, d2) => d1.SequenceEqual(d2),
              d => d.Aggregate(0, (k, v) => HashCode.Combine(k, v.GetHashCode())),
              d => d.ToList())
    {
    }
}