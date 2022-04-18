namespace Masa.Mc.Service.Admin.Infrastructure.Repositories
{
    public class ReceiverGroupRepository : Repository<McDbContext, ReceiverGroup>, IReceiverGroupRepository
    {
        public ReceiverGroupRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
        {
        }
    }
}
