﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Repositories;

public class WebsiteMessageCursorRepository : Repository<McDbContext, WebsiteMessageCursor>, IWebsiteMessageCursorRepository
{
    public WebsiteMessageCursorRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
