namespace MASA.MC.Service.Admin.Infrastructure.Repositories;

public class MessageTemplateRepository : Repository<MCDbContext, MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(MCDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<MessageTemplate>> GetQueryableAsync()
    {
        return await Task.FromResult(_context.Set<MessageTemplate>().AsQueryable());
    }
}
