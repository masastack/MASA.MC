// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public static class MessageRecordEfCoreQueryableExtensions
{
    public static IQueryable<MessageRecord> IncludeDetails(this IQueryable<MessageRecord> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x=>x.Channel);
    }

    public static IQueryable<MessageRecord> IncludeMessageTask(this IQueryable<MessageRecord> queryable)
    {
        return queryable.Include(x => x.MessageTask);
    }
}
