namespace Masa.Mc.Service.Infrastructure.Repositories;

public class ChannelRepository : Repository<MCDbContext, Channel>, IChannelRepository
{
    public ChannelRepository(MCDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<Channel>> GetQueryableAsync()
    {
        return await Task.FromResult(_context.Set<Channel>().AsQueryable());
    }
}
