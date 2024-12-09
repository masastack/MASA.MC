// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.Extensions;

public static class MasaDbContextBuilderExtensions
{
    public static MasaDbContextBuilder UseDbSql(this MasaDbContextBuilder builder, string dbType)
    {
        switch (dbType)
        {
            case "PostgreSql":
                McDbContext.RegisterAssembly(typeof(McPostgreSqlDbContextFactory).Assembly);
                builder.UseNpgsql(b => b.MigrationsAssembly("Masa.Mc.EntityFrameworkCore.PostgreSql"));
                break;
            default:
                McDbContext.RegisterAssembly(typeof(McSqlServerDbContextFactory).Assembly);
                builder.UseSqlServer(b => b.MigrationsAssembly("Masa.Mc.EntityFrameworkCore.SqlServer"));
                break;
        }
        return builder;
    }
}
