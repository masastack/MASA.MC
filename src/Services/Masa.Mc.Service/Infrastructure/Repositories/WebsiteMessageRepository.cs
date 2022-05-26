// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class WebsiteMessageRepository : Repository<McDbContext, WebsiteMessage>, IWebsiteMessageRepository
{
    public WebsiteMessageRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<IQueryable<WebsiteMessage>> GetQueryableAsync()
    {
        return await Task.FromResult(Context.Set<WebsiteMessage>().AsQueryable());
    }

    public async Task<IQueryable<WebsiteMessage>> WithDetailsAsync()
    {
        var query = await GetQueryableAsync();
        return query.IncludeDetails();
    }

    public async Task<WebsiteMessage?> FindAsync(Expression<Func<WebsiteMessage, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken))
    {
        return include
            ? await (await WithDetailsAsync()).Where(predicate).FirstOrDefaultAsync(cancellationToken)
            : await Context.Set<WebsiteMessage>().Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<WebsiteMessage>> GetChannelListAsync(Guid userId)
    {
        var set = Context.Set<WebsiteMessage>().AsNoTracking();
        var sorted = set.OrderByDescending(x => x.CreationTime);
        var list = set.Select(x => x.ChannelId)
            .Distinct()
            .SelectMany(x => sorted.Where(y => y.ChannelId == x).Take(1));
        return await Task.FromResult(list.OrderByDescending(x => x.CreationTime).ToList());
    }
}
