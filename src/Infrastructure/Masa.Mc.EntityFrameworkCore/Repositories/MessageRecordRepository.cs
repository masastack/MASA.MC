// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public class MessageRecordRepository : Repository<McDbContext, MessageRecord>, IMessageRecordRepository
{
    public MessageRecordRepository(McDbContext context, IUnitOfWork unitOfWork)
    : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<MessageRecord>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<MessageRecord>().AsQueryable());
    }

    public async Task<IQueryable<MessageRecord>> WithDetailsAsync()
    {
        var query = await GetQueryableAsync();
        return query.IncludeDetails();
    }

    public async Task<MessageRecord?> FindAsync(Expression<Func<MessageRecord, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
            : await Context.Set<MessageRecord>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsExistsAsync(Guid taskHistoryId, Guid userId)
    {
        return await Context.Set<MessageRecord>().AnyAsync(x => x.MessageTaskHistoryId == taskHistoryId && x.UserId == userId);
    }

    public async Task<bool> AnyAsync(Expression<Func<MessageRecord, bool>> predicate)
    {
        return await Context.Set<MessageRecord>().AnyAsync(predicate);
    }

    public async Task UpdateManyAsync(IEnumerable<MessageRecord> entities, bool autoSave = false, CancellationToken cancellationToken = default(CancellationToken))
    {
        var dbContext = Context.Set<MessageRecord>();
        await Context.BulkUpdateAsync(entities.ToList());
        if (autoSave)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
