using MASA.MC.Service.Admin.Domain.Channels.Aggregates;
using MASA.MC.Service.Admin.Domain.Channels.Repositories;

namespace MASA.MC.Service.Infrastructure.Repositories;

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
