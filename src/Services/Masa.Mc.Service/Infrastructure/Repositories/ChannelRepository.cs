namespace Masa.Mc.Service.Infrastructure.Repositories;

public class ChannelRepository : Repository<McDbContext, Channel>, IChannelRepository
{
    public ChannelRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<Channel>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<Channel>().AsQueryable());
    }
}
