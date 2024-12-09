// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore;

public class McPostgreSqlDbContextFactory : IDesignTimeDbContextFactory<McDbContext>
{
    public McDbContext CreateDbContext(string[] args)
    {
        McDbContext.RegisterAssembly(typeof(McPostgreSqlDbContextFactory).Assembly);
        var optionsBuilder = new MasaDbContextOptionsBuilder<McDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.PostgreSql.json")
            .Build();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection")!, b => b.MigrationsAssembly("Masa.Mc.EntityFrameworkCore.PostgreSql"));

        return new McDbContext(optionsBuilder.MasaOptions);
    }
}
