namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class MessageTemplateRepository : Repository<McDbContext, MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<MessageTemplate>> GetQueryableAsync()
    {
        return await Task.FromResult(_context.Set<MessageTemplate>().AsQueryable());
    }
}
