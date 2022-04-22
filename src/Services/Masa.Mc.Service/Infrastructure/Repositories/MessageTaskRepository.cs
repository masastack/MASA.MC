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
}
