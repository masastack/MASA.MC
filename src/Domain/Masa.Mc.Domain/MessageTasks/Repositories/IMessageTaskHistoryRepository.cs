// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.MessageTasks.Repositories;

public interface IMessageTaskHistoryRepository : IRepository<MessageTaskHistory>
{
    Task<IQueryable<MessageTaskHistory>> GetQueryableAsync();
    Task<IQueryable<MessageTaskHistory>> WithDetailsAsync();
    Task<MessageTaskHistory?> FindAsync(Expression<Func<MessageTaskHistory, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
    Task<bool> AnyAsync(Expression<Func<MessageTaskHistory, bool>> predicate);
    Task<MessageTaskHistory?> FindWaitSendAsync(Guid messageTaskId, bool isTest, CancellationToken cancellationToken = default(CancellationToken));
}
