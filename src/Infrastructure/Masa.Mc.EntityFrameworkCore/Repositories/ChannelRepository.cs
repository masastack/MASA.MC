// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Infrastructure.Repositories;

public class ChannelRepository : Repository<McDbContext, Channel>, IChannelRepository
{
    public ChannelRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }

    public async Task<Guid?> GetIdByCode(string code)
    {
        var channel = await Context.Set<MessageTemplate>().FirstOrDefaultAsync(x => x.Code == code);
        return channel?.Id;
    }

    public IQueryable<Channel> AsNoTracking()
    {
        return Context.Set<Channel>().AsNoTracking();
    }
}
