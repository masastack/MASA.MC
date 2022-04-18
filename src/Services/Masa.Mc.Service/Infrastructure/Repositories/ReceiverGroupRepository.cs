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
            return await Task.FromResult(_context.Set<ReceiverGroup>().AsQueryable());
        }

        private async Task<IQueryable<ReceiverGroup>> WithDetailsAsync()
        {
            var query = await GetQueryableAsync();
            return query.IncludeDetails();
        }

        public async Task<ReceiverGroup?> FindAsync(Expression<Func<ReceiverGroup, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            return include
                ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
                : await _context.Set<ReceiverGroup>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
