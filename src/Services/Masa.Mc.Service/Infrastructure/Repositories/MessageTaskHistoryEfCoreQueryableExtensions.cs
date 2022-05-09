// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public static class MessageTaskHistoryEfCoreQueryableExtensions
{
    public static IQueryable<MessageTaskHistory> IncludeDetails(this IQueryable<MessageTaskHistory> queryable, bool include = true)
    {
        if (!include)
        {
            return queryable;
        }
        return queryable.Include(x => x.MessageTask).Include(x=>x.ReceiverUsers);
    }
}
