// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.WebsiteMessages.Repositories;

public interface IWebsiteMessageRepository : IRepository<WebsiteMessage>
{
    Task<IQueryable<WebsiteMessage>> GetQueryableAsync();
    Task<IQueryable<WebsiteMessage>> WithDetailsAsync();
    Task<WebsiteMessage?> FindAsync(Expression<Func<WebsiteMessage, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));
    Task<List<WebsiteMessage>> GetChannelListAsync();
}
