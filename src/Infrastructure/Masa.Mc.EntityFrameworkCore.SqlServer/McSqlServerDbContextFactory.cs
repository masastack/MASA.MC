// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Alert.EntityFrameworkCore;

public class McSqlServerDbContextFactory : IDesignTimeDbContextFactory<McDbContext>
{
    public McDbContext CreateDbContext(string[] args)
    {
       McDbContext.RegisterAssembly(typeof(McSqlServerDbContextFactory).Assembly);
        var optionsBuilder = new MasaDbContextOptionsBuilder<McDbContext>();
        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.SqlServer.json")
            .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Masa.Mc.EntityFrameworkCore.SqlServer"));

        return new McDbContext(optionsBuilder.MasaOptions);
    }
}
