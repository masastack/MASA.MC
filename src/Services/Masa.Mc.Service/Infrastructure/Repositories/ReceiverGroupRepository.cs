// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories
{
    public class ReceiverGroupRepository : Repository<McDbContext, ReceiverGroup>, IReceiverGroupRepository
    {
        public ReceiverGroupRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
        {
        }

        private async Task<IQueryable<ReceiverGroup>> GetQueryableAsync()
        {
            return await Task.FromResult(Context.Set<ReceiverGroup>().AsQueryable());
        }

        public async Task<IQueryable<ReceiverGroup>> WithDetailsAsync()
        {
            var query = await GetQueryableAsync();
            return query.IncludeDetails();
        }

        public async Task<ReceiverGroup?> FindAsync(Expression<Func<ReceiverGroup, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return include
                ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
                : await Context.Set<ReceiverGroup>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
