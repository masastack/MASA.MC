// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class MessageRecordRepository : Repository<McDbContext, MessageRecord>, IMessageRecordRepository
{
    public MessageRecordRepository(McDbContext context, IUnitOfWork unitOfWork)
    : base(context, unitOfWork)
    {
    }
}
