// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageRecords.Repositories;

public interface IMessageRecordRepository : IRepository<MessageRecord>
{
    Task<IQueryable<MessageRecord>> GetQueryableAsync();

    Task<IQueryable<MessageRecord>> WithDetailsAsync();

    Task<MessageRecord?> FindAsync(Expression<Func<MessageRecord, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> IsExistsAsync(Guid taskHistoryId, Guid userId);

    Task<bool> AnyAsync(Expression<Func<MessageRecord, bool>> predicate);

    Task UpdateManyAsync(IEnumerable<MessageRecord> entities, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken));
}
