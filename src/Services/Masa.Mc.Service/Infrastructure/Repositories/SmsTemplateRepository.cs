// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class SmsTemplateRepository : Repository<McDbContext, SmsTemplate>, ISmsTemplateRepository
{
    public SmsTemplateRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
