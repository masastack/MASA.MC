// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public static class MessageTaskEfCoreQueryableExtensions
{
    public static IQueryable<MessageTask> IncludeDetails(this IQueryable<MessageTask> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x=>x.Channel).Include(x => x.Historys);
    }
}
