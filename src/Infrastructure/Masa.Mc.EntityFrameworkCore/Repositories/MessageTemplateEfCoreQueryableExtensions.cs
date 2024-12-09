// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public static class MessageTemplateEfCoreQueryableExtensions
{
    public static IQueryable<MessageTemplate> IncludeDetails(this IQueryable<MessageTemplate> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x => x.Items);
    }
}
