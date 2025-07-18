// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Mc.EntityFrameworkCore;

public class McDbContext : MasaDbContext<McDbContext>
{
    internal static Assembly Assembly = typeof(McDbContext).Assembly;

    public DbSet<IntegrationEventLog> EventLogs { get; set; }

    public McDbContext(MasaDbContextOptions<McDbContext> options) : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
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
            b.Property(x => x.Scheme).HasMaxLength(128).HasColumnName(nameof(Channel.Scheme));
            b.Property(x => x.SchemeField).HasMaxLength(128).HasColumnName(nameof(Channel.SchemeField));
            b.Property(x => x.Provider).HasColumnName(nameof(Channel.Provider));
            b.HasOne<Channel>().WithOne().HasForeignKey<AppChannel>(x => x.Id);
        });

        builder.ConfigureMC();

        builder.ApplyConfiguration(new IntegrationEventLogEntityTypeConfiguration());
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()!);

        // Apply provider-specific configurations
        ApplyProviderSpecificConfigurations(builder);
    }

    public static void RegisterAssembly(Assembly assembly)
    {
        Assembly = assembly;
    }

    private void ApplyProviderSpecificConfigurations(ModelBuilder builder)
    {
        if (Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new DateTimeUtcConverter());
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new NullableDateTimeUtcConverter());
                    }
                }
            }
        }
    }
}
