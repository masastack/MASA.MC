namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Repositories;

public interface IMessageTemplateRepository : IRepository<MessageTemplate>
{
    Task<MessageTemplate?> FindAsync(Expression<Func<MessageTemplate, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));

    Task<IQueryable<MessageTemplateWithDetail>> GetWithDetailQueryAsync();
}
