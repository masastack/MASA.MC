// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public class MessageTemplateRepository : Repository<McDbContext, MessageTemplate>, IMessageTemplateRepository
{
    public MessageTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    private IQueryable<MessageTemplate> AsQueryable()
    {
        return Context.Set<MessageTemplate>().AsQueryable();
    }

    private async Task<IQueryable<MessageTemplate>> WithDetailsAsync()
    {
        var query = AsQueryable();
        return query.IncludeDetails();
    }

    public async Task<MessageTemplate?> FindAsync(Expression<Func<MessageTemplate, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
            : await Context.Set<MessageTemplate>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(Expression<Func<MessageTemplate, bool>> predicate)
    {
        return await Context.Set<MessageTemplate>().AnyAsync(predicate);
    }

    public IQueryable<MessageTemplate> AsNoTracking()
    {
        return Context.Set<MessageTemplate>().AsNoTracking();
    }
}

