// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Repositories;

public interface IMessageTaskRepository : IRepository<MessageTask>
{
    Task<IQueryable<MessageTask>> WithDetailsAsync();
    Task<MessageTask?> FindAsync(Expression<Func<MessageTask, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
}
