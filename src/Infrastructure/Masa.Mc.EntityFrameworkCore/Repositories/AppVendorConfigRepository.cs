// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore.Repositories;

public class AppVendorConfigRepository : Repository<McDbContext, AppVendorConfig>, IAppVendorConfigRepository
{
    public AppVendorConfigRepository(McDbContext context, IUnitOfWork unitOfWork)
        : base(context, unitOfWork)
    {
    }
}
