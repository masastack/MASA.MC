// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public class UnsubscriptionRepository : Repository<McDbContext, Unsubscription>, IUnsubscriptionRepository
{
    public UnsubscriptionRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<Unsubscription?> FindActiveAsync(
        Guid channelId,
        string channelUserIdentity,
        UnsubscriptionScopeTypes scopeType,
        string scopeRefId,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Unsubscription>()
            .Include(x => x.Timelines)
            .Where(x =>
                x.ChannelId == channelId &&
                x.ChannelUserIdentity == channelUserIdentity &&
                x.ScopeType == scopeType &&
                x.ScopeRefId == scopeRefId &&
                x.Status == UnsubscriptionStatus.Unsubscribed);

        return await query.OrderByDescending(x => x.UnsubscribedAt).FirstOrDefaultAsync(cancellationToken);
    }
}
