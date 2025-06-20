﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public class AppDeviceTokenRepository : Repository<McDbContext, AppDeviceToken>, IAppDeviceTokenRepository
{
    public AppDeviceTokenRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
