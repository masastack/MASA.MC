// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Infrastructure.Repositories;

public class MessageTaskHistoryRepository : Repository<McDbContext, MessageTaskHistory>, IMessageTaskHistoryRepository
{
    public MessageTaskHistoryRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<MessageTaskHistory>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<MessageTaskHistory>().AsQueryable());
    }

    public async Task<IQueryable<MessageTaskHistory>> WithDetailsAsync()
    {
        var query = await GetQueryableAsync();
        return query.IncludeDetails();
    }

    public async Task<MessageTaskHistory?> FindAsync(Expression<Func<MessageTaskHistory, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
            : await Context.Set<MessageTaskHistory>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }
}
