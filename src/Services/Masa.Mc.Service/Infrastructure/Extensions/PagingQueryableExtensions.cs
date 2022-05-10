// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System.Linq;

public static class PagingQueryableExtensions
{
    public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int page, int pageSize)
    {
        return query.Skip((page - 1) * pageSize).Take(pageSize <= 0 ? int.MaxValue : pageSize);
    }
}

