// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Domain.MessageTemplates.Repositories;

public interface IMessageTemplateRepository : IRepository<MessageTemplate>
{
    Task<MessageTemplate?> FindAsync(Expression<Func<MessageTemplate, bool>> predicate, bool include = true, CancellationToken cancellationToken = default(CancellationToken));

    Task<bool> AnyAsync(Expression<Func<MessageTemplate, bool>> predicate);
}
