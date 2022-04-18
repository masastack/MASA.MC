namespace Masa.Mc.Service.Admin.Domain.ReceiverGroups.Repositories;

public interface IReceiverGroupRepository : IRepository<ReceiverGroup>
{
    Task<ReceiverGroup?> FindAsync(Expression<Func<ReceiverGroup, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
}
