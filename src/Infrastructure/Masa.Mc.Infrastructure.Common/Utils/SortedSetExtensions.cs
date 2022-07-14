// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Infrastructure.Common.Utils;

internal static class SortedSetExtensions
{
    internal static SortedSet<int> TailSet(this SortedSet<int> set, int value)
    {
        return set.GetViewBetween(value, 9999999);
    }
}
