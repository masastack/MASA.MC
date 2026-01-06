// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Channels.Repositories;

public interface IChannelRepository : IRepository<Channel>
{
    Task<Guid?> GetIdByCode(string code);
    
    IQueryable<Channel> AsNoTracking();
}
