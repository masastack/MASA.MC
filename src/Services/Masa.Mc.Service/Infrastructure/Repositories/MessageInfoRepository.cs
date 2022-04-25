namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class MessageInfoRepository : Repository<McDbContext, MessageInfo>, IMessageInfoRepository
{
    public MessageInfoRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
