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

    public async Task<bool> AnyAsync(Expression<Func<MessageTaskHistory, bool>> predicate)
    {
        return await Context.Set<MessageTaskHistory>().AnyAsync(predicate);
    }

    public async Task<MessageTaskHistory?> FindWaitSendAsync(Guid messageTaskId, bool isTest, CancellationToken cancellationToken = default(CancellationToken))
    {
        return await (await WithDetailsAsync()).Where(x => x.IsTest == isTest && x.MessageTaskId == messageTaskId && x.Status == MessageTaskHistoryStatuses.WaitSend).OrderBy(x => x.CreationTime).FirstOrDefaultAsync(cancellationToken);
    }
}
