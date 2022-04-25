namespace Masa.Mc.Service.Admin.Domain.MessageTasks.Repositories;

public interface IMessageTaskRepository : IRepository<MessageTask>
{
    Task<IQueryable<MessageTask>> WithDetailsAsync();
    Task<MessageTask?> FindAsync(Expression<Func<MessageTask, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
}
