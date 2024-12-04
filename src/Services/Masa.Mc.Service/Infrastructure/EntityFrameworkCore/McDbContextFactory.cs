// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

public class McDbContextFactory : IDesignTimeDbContextFactory<McDbContext>
{
    public McDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new MasaDbContextOptionsBuilder<McDbContext>();

        var configurationBuilder = new ConfigurationBuilder();
        var configuration = configurationBuilder
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

        return new McDbContext(optionsBuilder.MasaOptions);
    }
}
