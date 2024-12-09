// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.ReceiverGroups.Repositories;

public interface IReceiverGroupRepository : IRepository<ReceiverGroup>
{
    Task<IQueryable<ReceiverGroup>> WithDetailsAsync();
    Task<ReceiverGroup?> FindAsync(Expression<Func<ReceiverGroup, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
}
