// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Infrastructure.Repositories;

public class MessageTaskRepository : Repository<McDbContext, MessageTask>, IMessageTaskRepository
{
    public MessageTaskRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<MessageTask>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<MessageTask>().AsQueryable());
    }

    public async Task<IQueryable<MessageTask>> WithDetailsAsync()
    {
        var query = await GetQueryableAsync();
        return query.IncludeDetails();
    }

    public async Task<MessageTask?> FindAsync(Expression<Func<MessageTask, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).AsTracking().FirstOrDefaultAsync(cancellationToken)
            : await Context.Set<MessageTask>().Where(predicate).AsTracking().FirstOrDefaultAsync(cancellationToken);
    }
}
