// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Domain.Unsubscriptions.Repositories;

public interface IUnsubscriptionRepository : IRepository<Unsubscription>
{
    Task<Unsubscription?> FindActiveAsync(
        Guid channelId,
        string channelUserIdentity,
        UnsubscriptionScopeTypes scopeType,
        string scopeRefId,
        CancellationToken cancellationToken = default);
}
