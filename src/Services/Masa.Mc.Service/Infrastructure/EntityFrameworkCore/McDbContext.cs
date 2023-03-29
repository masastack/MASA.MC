// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.Service.Admin.Infrastructure.EntityFrameworkCore;

[ConnectionStringName("DefaultConnection")]
public class McDbContext : MasaDbContext<McDbContext>
{
    public DbSet<IntegrationEventLog> EventLogs { get; set; }
    public McDbContext(MasaDbContextOptions<McDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        base.OnModelCreatingExecuting(builder);

        builder.Entity<AppChannel>(b =>
        {
            b.ToTable(MCConsts.DbTablePrefix + "Channels", MCConsts.DbSchema);
            b.Property(x => x.Color).HasMaxLength(128).HasColumnName(nameof(Channel.Color));
            b.Property(x => x.Code).IsRequired().HasMaxLength(64).HasColumnName(nameof(Channel.Code));
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(128).HasColumnName(nameof(Channel.DisplayName));
            b.Property(x => x.Type).HasConversion(x => x.Id, x => Enumeration.FromValue<ChannelType>(x)).HasColumnName(nameof(Channel.Type));
            b.HasOne<Channel>().WithOne().HasForeignKey<AppChannel>(x => x.Id);
        });

        builder.ConfigureMC();

        builder.ApplyConfiguration(new IntegrationEventLogEntityTypeConfiguration());
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()!);
    }
}
