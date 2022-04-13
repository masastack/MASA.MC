using System.Linq.Dynamic.Core;
namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class MessageTemplateRepository : Repository<McDbContext, MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    private async Task<IQueryable<MessageTemplate>> GetQueryableAsync()
    {
        return await Task.FromResult(_context.Set<MessageTemplate>().AsQueryable());
    }

    private async Task<IQueryable<MessageTemplate>> WithDetailsAsync()
    {
        var query = await GetQueryableAsync();
        return query.IncludeDetails();
    }

    public async Task<MessageTemplate?> FindAsync(Expression<Func<MessageTemplate, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
            : await _context.Set<MessageTemplate>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IQueryable<MessageTemplateWithDetail>> GetWithDetailQueryAsync()
    {
        var templateSet = await WithDetailsAsync();
        var channelSet = _context.Set<Channel>();
        var query = from messageTemplate in templateSet
                    join channel in channelSet
                    on messageTemplate.ChannelId equals channel.Id into channelJoined
                    from channel in channelJoined.DefaultIfEmpty()
                    select new MessageTemplateWithDetail
                    {
                        MessageTemplate = messageTemplate,
                        Channel = channel
                    };
        return query;
    }
}

