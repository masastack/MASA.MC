// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class MessageTemplateRepository : Repository<McDbContext, MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    private async Task<IQueryable<MessageTemplate>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<MessageTemplate>().AsQueryable());
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
            : await Context.Set<MessageTemplate>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IQueryable<MessageTemplateWithDetail>> GetWithDetailQueryAsync()
    {
        var templateSet = await WithDetailsAsync();
        var channelSet = Context.Set<Channel>();
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

    public async Task<bool> AnyAsync(Expression<Func<MessageTemplate, bool>> predicate)
    {
        return await Context.Set<MessageTemplate>().AnyAsync(predicate);
    }
}

